using System;
using System.Runtime.Serialization;

namespace Campfire.Api.Models
{
    [DataContract]
    public class User
    {      
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "email_address")]
        public string EmailAddress { get; set; }
        [DataMember]
        public bool Admin { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public string Type { get; set; }

        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
