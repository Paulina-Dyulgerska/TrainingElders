namespace RabbitMQChatClient
{
    public static class UI
    {
        public static class Cli
        {
            public static string GetHost()
            {
                Console.WriteLine("Enter service IP (default: localhost): ");
                var ip = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(ip))
                    ip = "localhost";

                return ip;
            }

            public static string GetUsername()
            {
                string? username = string.Empty;
                while (string.IsNullOrWhiteSpace(username))
                {
                    Console.Write("Enter your username: ");
                    username = Console.ReadLine();
                }

                return username;
            }
        }
    }
}
