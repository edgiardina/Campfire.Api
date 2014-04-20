using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Campfire.Api.Models
{
    [DataContract]
    public class Tweet
    {
        [DataMember(Name = "author_avatar_url")]
        public string AuthorAvatarUrl { get; set; }

        [DataMember(Name = "author_username")]
        public string AuthorUsername { get; set; }

        [DataMember]
        public ulong Id { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
