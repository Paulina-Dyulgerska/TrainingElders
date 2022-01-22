using System;
using System.Runtime.Serialization;

namespace WcfServiceLibrary.Models
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Time { get; set; }
    }
}
