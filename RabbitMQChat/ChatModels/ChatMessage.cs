using System;

namespace ChatModels
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

        //public ChatMessage(string author, string content, DateTimeOffset createdOn, string receiverName) : this(Guid.NewGuid().ToString(), author, content, createdOn)
        //{
        //    if (receiverName != null)
        //    {
        //        Receiver = new Client(receiverName);
        //    }
        //}

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
                    IsForAll = chatMessage.IsForAll,
                    ReceiverName = chatMessage.Receiver?.Username,
                };

                return chatMessageDto;
            }

            public ChatMessage ToModel()
            {
                var model = new ChatMessage(Author, Content, CreatedOn);
                if (string.IsNullOrWhiteSpace(ReceiverName) == false)
                {
                    model.To(new Client(ReceiverName));
                }
                return model;
            }

            public string Id { get; set; }

            public string Author { get; set; }

            public string Content { get; set; }

            public DateTimeOffset CreatedOn { get; set; }

            public bool IsForAll { get; set; }

            public string ReceiverName { get; set; }
        }
    }
}
