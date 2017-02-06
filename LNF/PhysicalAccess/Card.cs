using System;

namespace LNF.PhysicalAccess
{
    public class Card
    {
        public object ID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Number { get; set; }
        public DateTime CardIssueDate { get; set; }
        public DateTime CardExpireDate { get; set; }
        public DateTime BadgeIssueDate { get; set; }
        public DateTime BadgeExpireDate { get; set; }
        public DateTime? LastAccess { get; set; }
        public Status Status { get; set; }
    }
}
