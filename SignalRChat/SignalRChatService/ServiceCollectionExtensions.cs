using Microsoft.Extensions.DependencyInjection;
using SignalRChatModels;

namespace SignalRChatService
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChatApp(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionStore>();
            services.AddSingleton<ChatRoom>();
            services.AddTransient<IChatCommunicationChannel, SignalRCommunicationChannel>();
            services.AddTransient<ChatRoomApplicationService>();

            return services;
        }
    }
}