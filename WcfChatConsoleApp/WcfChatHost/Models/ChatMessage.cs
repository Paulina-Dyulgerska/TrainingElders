using System;
using System.Runtime.Serialization;

namespace WcfChatHost.Models
{
    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime Time { get; set; }
    }
}
