using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Campfire.Api.Models
{
    [DataContract]
    public class SoundMessage : Message
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
