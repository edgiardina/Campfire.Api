using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Campfire.Api.Models
{
    [DataContract]
    public class MessageCollection
    {
        [DataMember]
        public List<Message> Messages { get; set; } 
    }

    [DataContract]
    public class SingleMessage
    {
        [DataMember]
        public Message Message { get; set; }
    }
}
