using System;

namespace SignalRChatModels
{
    public class ChatMessage
    {
        public ChatMessage(string author, string content)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException($"'{nameof(author)}' cannot be null or whitespace.", nameof(author));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException($"'{nameof(content)}' cannot be null or whitespace.", nameof(content));

            Id = Guid.NewGuid().ToString();
            Author = author;
            Content = content;
            CreatedOn = DateTimeOffset.UtcNow;
        }

        public ChatMessage(string author, string content, DateTimeOffset createdOn) : this(author, content)
        {
            CreatedOn = createdOn;
        }

        public string Id { get; }

        public string Author { get; }

        public string Content { get; }

        public DateTimeOffset CreatedOn { get; }
    }
}
