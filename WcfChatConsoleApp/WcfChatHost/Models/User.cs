using System;
using System.Runtime.Serialization;

namespace WcfChatHost.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Time { get; set; }
    }
}
