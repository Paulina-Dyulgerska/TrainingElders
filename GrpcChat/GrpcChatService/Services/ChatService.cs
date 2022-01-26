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
        private static readonly object mutex = new object();
        private static readonly IList<Models.User> users = new List<Models.User>();
        private readonly ILogger<ChatService> logger;

        public ChatService(ILogger<ChatService> logger)
        {
            this.logger = logger;
        }

        public override async Task<ConnectUserResponse> Connect(ConnectUserRequest request, ServerCallContext context)
        {
            var newUser = new Models.User
            {
                Id = request.User.Id,
                Name = request.User.Name,
            };

            var response = new ConnectUserResponse();

            try
            {
                await SendMessageToAllUsers(new ChatMessage { UserName = "Chat Service", Message = $"User {newUser.Name} joined the chat!" });
                lock (mutex)
                {
                    users.Add(newUser);
                }
                response.IsUserConnected = true;
                response.Message = $"User {newUser.Name} connected!";
            }
            catch (Exception ex)
            {
                users.Remove(newUser);
                response.Message = $"Failed to connect {newUser.Name}!";
                throw ex;
            }

            return response;
        }

        public override async Task SendMessageToChatService(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            logger.LogInformation($"Connection id: {httpContext.Connection.Id}");

            if (await requestStream.MoveNext() == false)
                return;

            var userId = requestStream.Current.UserId;
            var user = users.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
                user.responseStream = responseStream;

            logger.LogInformation($"{user.Name} ({user.Id}) connected.");

            try
            {
                while (await requestStream.MoveNext())
                {
                    if (!string.IsNullOrEmpty(requestStream.Current.Message))
                    {
                        if (string.Equals(requestStream.Current.Message, "exit", StringComparison.OrdinalIgnoreCase))
                        {
                            DisconnectUser(requestStream.Current.UserId);
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
                await user.responseStream.WriteAsync(chatMessage);
                logger.LogInformation($"Sent message from {chatMessage.UserName} to {user.Name}.");
            }
        }

        private async void DisconnectUser(string userId)
        {
            var user = users.Where(x => x.Id == userId).FirstOrDefault();
            users.Remove(user);
            logger.LogInformation($"{user.Name} ({user.Id}) disconnected.");
            await SendMessageToAllUsers(new ChatMessage { UserName = "Chat Service", Message = $"User {user.Name} left the chat!" });
        }
    }
}
