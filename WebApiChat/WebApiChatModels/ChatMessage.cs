using System;

namespace WebApiChatModels
{
    public class ChatMessage
    {
        public ChatMessage()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; }

        public string UserName { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
