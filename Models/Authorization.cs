using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Campfire.Api.Models
{
    //Set of classes that represent the Authorization object returned during OAuth to Basecamp
    [DataContract]
    public class Identity
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }
        [DataMember(Name = "last_name")]
        public string LastName { get; set; }
        [DataMember(Name = "email_address")]
        public string EmailAddress { get; set; }
    }

    [DataContract]
    public class Account
    {
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Href { get; set; }
    }

    [DataContract]
    public class Authorization
    {
        [DataMember(Name = "expires_at")]
        public DateTime ExpiresAt { get; set; }
        [DataMember]
        public Identity Identity { get; set; }
        [DataMember]
        public List<Account> Accounts { get; set; }

        public string AccessToken { get; set; }
    }
}
