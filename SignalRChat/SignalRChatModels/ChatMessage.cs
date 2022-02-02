using System;

namespace SignalRChatModels
{
    public class ChatMessage
    {
        ChatMessage(string id, string author, string content, DateTimeOffset createdOn)
        {
            if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException($"'{nameof(author)}' cannot be null or whitespace.", nameof(author));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException($"'{nameof(content)}' cannot be null or whitespace.", nameof(content));

            Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
            Author = author;
            Content = content;
            CreatedOn = createdOn == DateTimeOffset.MinValue ? DateTimeOffset.UtcNow : createdOn;
        }

        public ChatMessage(string author, string content) : this(Guid.NewGuid().ToString(), author, content, DateTimeOffset.UtcNow) { }

        public ChatMessage(string author, string content, DateTimeOffset createdOn) : this(Guid.NewGuid().ToString(), author, content, createdOn) { }

        public bool IsForAll => Receiver is null;

        public string Id { get; private set; }

        public string Author { get; private set; }

        public Client Receiver { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedOn { get; private set; }

        public ChatMessage To(Client receiver)
        {
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            return this;
        }

        public ChatMessage ToAll()
        {
            Receiver = default;
            return this;
        }

        public class Dto
        {
            public static Dto From(ChatMessage chatMessage)
            {
                var chatMessageDto = new Dto
                {
                    Id = chatMessage.Id,
                    Author = chatMessage.Author,
                    Content = chatMessage.Content,
                    CreatedOn = chatMessage.CreatedOn,
                };

                return chatMessageDto;
            }

            public ChatMessage ToModel()
            {
                return new ChatMessage(Id, Author, Content, CreatedOn);
            }

            public string Id { get; set; }

            public string Author { get; set; }

            public string Content { get; set; }

            public DateTimeOffset CreatedOn { get; set; }
        }
    }
}
