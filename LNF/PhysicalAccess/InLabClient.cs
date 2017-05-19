using System;

namespace LNF.PhysicalAccess
{
    public class InLabClient
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public string AccessMethod { get; set; }
        public DateTime EventDateTime { get; set; }
        public double Duration { get; set; }

        public InLabClient() { }

        public InLabClient(Badge badge)
        {
            ClientID = badge.ClientID;
            UserName = badge.UserName;
            FullName = string.Format("{0} {1}", badge.FirstName, badge.LastName);
            FirstName = badge.FirstName;
            LastName = badge.LastName;
            CardNumber = badge.CurrentCardNumber.Value.ToString();
            AccessMethod = (badge.CurrentCardNumber.Value <= 999999) ? "Pin Code" : "Badge";
            EventDateTime = badge.CurrentAccessTime.Value;
            Duration = (DateTime.Now - badge.CurrentAccessTime.Value).TotalHours;
        }
    }
}
