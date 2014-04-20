using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Campfire.Api.Models
{
    [DataContract]
    public class TweetMessage : Message
    {
        [DataMember]
        public Tweet Tweet { get; set; }
    }
}
