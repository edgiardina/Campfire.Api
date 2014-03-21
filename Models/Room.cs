using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Campfire.Api.Models
{
    [DataContract]
    public class Room
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        public string Image {
            get { return "Assets/campfire.png"; }
        }
        [DataMember]
        public string Topic { get; set; }
        [DataMember(Name ="membership_limit")]
        public int MembershipLimit { get; set; }
        [DataMember]
        public bool Full { get; set; }
        [DataMember]
        public bool Locked { get; set; }
        [DataMember(Name = "open_to_guests")]
        public bool OpenToGuests { get; set; }
        [DataMember(Name = "updated_at")]
        public DateTime UpdatedAt { get; set; }
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "active_token_value")]
        public string ActiveTokenValue { get; set; }

         [DataMember(Name = "users")]
        public List<User> Users { get; set; } 

    }
}
