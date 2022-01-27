using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcChatService.Services
{
    public class ChatService : Chat.ChatBase
    {
        private static readonly ConcurrentDictionary<Models.User, IServerStreamWriter<ChatMessage>> users = new ConcurrentDictionary<Models.User, IServerStreamWriter<ChatMessage>>();
        private readonly ILogger<ChatService> logger;

        public ChatService(ILogger<ChatService> logger)
        {
            this.logger = logger;
        }

        public override async Task<ConnectUserResponse> Connect(ConnectUserRequest request, ServerCallContext context)
        {
            var msg = new ChatMessage
            {
                UserId = request.User.Id,
                UserName = request.User.Name,
            };

            try
            {
                await SendMessageToAllUsers(new ChatMessage { UserName = "Chat Service", Message = $"User {request.User.Name} joined the chat!" });
                users.TryAdd(new Models.User
                {
                    Id = request.User.Id,
                    Name = request.User.Name
                }, null);

                msg.Message = $"User {request.User.Name} connected!";
            }
            catch (Exception)
            {
                msg.Message = $"Failed to connect {request.User.Name}!";
                throw;
            }

            return new ConnectUserResponse
            {
                Message = msg.Message,
                IsUserConnected = true
            };
        }

        public override async Task SendMessageToChatService(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            logger.LogInformation($"Connection id: {httpContext.Connection.Id}");

            if (await requestStream.MoveNext() == false)
                return;

            var userId = requestStream.Current.UserId;
            var user = users.Where(x => x.Key.Id == userId).FirstOrDefault().Key;
            users.AddOrUpdate(user, x => responseStream, (u, x) => responseStream);

            logger.LogInformation($"{user.Name} ({user.Id}) connected.");

            try
            {
                while (await requestStream.MoveNext())
                {
                    if (string.IsNullOrEmpty(requestStream.Current.Message))
                        continue;

                    if (string.Equals(requestStream.Current.Message, "exit", StringComparison.OrdinalIgnoreCase))
                    {
                        DisconnectUser(requestStream.Current.UserId);
                        break;
                    }

                    await SendMessageToAllUsers(requestStream.Current);
                }
            }
            catch (IOException)
            {
                DisconnectUser(requestStream.Current.UserId);
                logger.LogInformation($"Connection for {user.Name} ({user.Id}) was aborted.");
            }

            return;
        }

        private async Task SendMessageToAllUsers(ChatMessage chatMessage)
        {
            foreach (var user in users)
            {
                await user.Value.WriteAsync(chatMessage);
                logger.LogInformation($"Sent message from {chatMessage.UserName} to {user.Key.Name}.");
            }
        }

        private async void DisconnectUser(string userId)
        {
            var user = users.Where(x => x.Key.Id == userId).FirstOrDefault();
            users.TryRemove(user.Key, out _);
            logger.LogInformation($"{user.Key.Name} ({user.Key.Id}) disconnected.");

            await SendMessageToAllUsers(new ChatMessage { UserName = "Chat Service", Message = $"User {user.Key.Name} left the chat!" });
        }
    }
}
