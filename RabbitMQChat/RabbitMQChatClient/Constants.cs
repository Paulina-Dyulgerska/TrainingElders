namespace RabbitMQChatClient
{
    public static class Constants
    {
        public static string NewUserQueueName => "newUser";
        public static string MessageToAllExchangeType = "messageExchange";
        public static string DirectMessageExchangeType = "direct";
    }
}
