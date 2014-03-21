using System;
using System.Runtime.Serialization;

namespace Campfire.Api.Models
{
    [DataContract]
    public class Upload
    {
        [DataMember(Name = "byte_size")]
        public int ByteSize { get; set; }
        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "room_id")]
        public int RoomId { get; set; }
        [DataMember(Name = "user_id")]
        public int UserId { get; set; }
        [DataMember(Name = "full_url")]
        public string FullUrl { get; set; }

        public string ThumbUrl
        {
            get { return FullUrl.Replace("uploads", "thumb"); }
        }
    }
}
