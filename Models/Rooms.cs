using System.Collections.Generic;

namespace Campfire.Api.Models
{
    public class RoomCollection
    {
        public List<Room> Rooms { get; set; } 
    }

    public class SingleRoom
    {
        public Room Room { get; set; }
    }
}
