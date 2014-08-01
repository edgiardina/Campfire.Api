using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Campfire.Api.Models
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember(Name = "room_id")]
        public int RoomId { get; set; }

        [DataMember(Name = "user_id")]
        public int? UserId { get; set; }
        public string UserName { get; set; }

        public string AvatarUrl { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }
        
        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public MessageType Type { get; set; }

        [DataMember]
        public bool Starred { get; set; }

        public bool IsStarrable
        {
            get 
            { 
                return this.Type == MessageType.TextMessage || this.Type == MessageType.PasteMessage || this.Type == MessageType.UploadMessage; 
            }
        }
    }
}
