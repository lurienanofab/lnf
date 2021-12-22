using System;
using System.Collections.Generic;

namespace LNF.PhysicalAccess
{
    public class BadgeInArea
    {
        public string AreaName { get; set; }
        public string AreaDisplayName { get; set; }
        public IEnumerable<BadgeInAreaOccupant> Occupants { get; set; }
    }

    public class BadgeInAreaOccupant
    {
        public int ClientID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AreaName { get; set; }
        public DateTime? AccessTime { get; set; }
        public int? CardNumber { get; set; }
    }
}
