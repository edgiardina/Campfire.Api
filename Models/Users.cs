using System.Collections.Generic;

namespace Campfire.Api.Models
{
    public class Users
    {
        public List<User> User { get; set; }
    }

    public class SingleUser
    {
        public User User { get; set; }
    }
}
