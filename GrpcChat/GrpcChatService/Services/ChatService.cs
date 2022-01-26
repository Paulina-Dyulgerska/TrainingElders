using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcChatService.Services
{
    public class ChatService : Chat.ChatBase
    {
        private readonly ILogger<ChatService> logger;

        private static readonly IList<Models.User> users = new List<Models.User>();

        public ChatService(ILogger<ChatService> logger)
        {
            this.logger = logger;
        }

        public override async Task<ConnectUserResponse> Connect(ConnectUserRequest request, ServerCallContext context)
        {
            var hasThisUser = users.Any(x => x.Id == request.User.Id);

            var response = new ConnectUserResponse();
            if (hasThisUser != true)
            {
                var currentUser = new Models.User
                {
                    Id = request.User.Id,
                    Name = request.User.Name,
                };

                try
                {
                    await SendMessageToAllUsers(new ChatMessage { Message = $"User {currentUser.Name} joined the chat!", UserName = "ChatService" });
                }
                catch
                {
                    users.Remove(currentUser);
                    response.IsUserConnected = false;
                    response.Message = $"Failed to connect {currentUser.Name}!";
                }

                users.Add(currentUser);
                response.IsUserConnected = true;
                response.Message = $"Hello, {currentUser.Name}!";
            }
            else
            {
                response.Message = $"Already has such user {request.User.Name} ({request.User.Id})!";
            }

            return response;
        }

        public override async Task SendMessageToChatService(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            logger.LogInformation($"Connection id: {httpContext.Connection.Id}");

            if (!await requestStream.MoveNext())
            {
                return;
            }

            var userId = requestStream.Current.UserId;
            var user = users.Where(x => x.Id == userId).FirstOrDefault();

            if (user == null)
            { } //TODO
            user.Stream = responseStream;

            logger.LogInformation($"{user} connected");

            try
            {
                while (await requestStream.MoveNext())
                {
                    if (!string.IsNullOrEmpty(requestStream.Current.Message))
                    {
                        if (string.Equals(requestStream.Current.Message, "exit", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                        await SendMessageToAllUsers(requestStream.Current);
                    }
                }
            }
            catch (IOException)
            {
                DisconnectUser(requestStream.Current.UserId);
                logger.LogInformation($"Connection for {user.Name} ({user.Id}) was aborted.");
            }
        }

        private async Task SendMessageToAllUsers(ChatMessage chatMessage)
        {

            foreach (var user in users)
            {
                await user.Stream.WriteAsync(chatMessage);
                logger.LogInformation($"Sent message from {chatMessage.UserName} to {user.Name}");
            }
        }

        private void DisconnectUser(string userId)
        {
            var user = users.Where(x => x.Id == userId).FirstOrDefault();
            users.Remove(user);
        }
    }
}
