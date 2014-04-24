using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campfire.Api.Models
{
    public class UploadMessage : Message
    {
        public Upload Upload { get; set; }

        public new string ImageUrl
        {
            get
            {
                //TODO: Return null if fullUrl doesn't match image rules.
                return this.Upload.FullUrl;
            }
        }
    }
}
