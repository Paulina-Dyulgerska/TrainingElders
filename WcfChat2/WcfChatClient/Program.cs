using System;

namespace WcfChatClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var chatClient = new ChatClient(new System.ServiceModel.InstanceContext(null));
        }
    }
}
