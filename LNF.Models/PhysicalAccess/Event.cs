using System;
using System.Linq;

namespace LNF.Models.PhysicalAccess
{
    public class Event
    {
        public string ID { get; set; }
        public string DeviceID { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public EventType EventType { get; set; }
        public string UserName { get; set; }
        public string LastName { get;set;}
        public string FirstName { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventDescription { get; set; }
        public string DeviceDescription { get; set; }
        public int CardNumber { get; set; }
        public Status CardStatus { get;set;}
        public DateTime CardIssueDate { get; set; }
        public DateTime CardExpireDate { get; set; }

        public bool IsAntipassbackError()
        {
            return new[] { EventType.AntipassbackErrorIn, EventType.AntipassbackErrorOut }.Contains(EventType);
        }
    }
}
