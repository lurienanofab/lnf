using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Helpdesk
{
    public class Ticket
    {
        public string TicketID { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AssignedTo { get; set; }
        public int ResourceID { get; set; }

        public static IList<Ticket> SearchByEmail(string Email, string urlBase, string apiKey)
        {
            IList<Ticket> result = new List<Ticket>();
            string[] splitter = Email.Split('@');

            DataTable dtTickets = new LNF.Helpdesk.Service(urlBase, apiKey).SelectTicketsByEmail(splitter[0] + '%');

            foreach (DataRow dr in dtTickets.Rows)
            {
                result.Add(new Ticket
                {
                    TicketID = dr["ticketID"].ToString(),
                    Email = dr["email"].ToString(),
                    Subject = dr["subject"].ToString(),
                    CreatedOn = DateTime.Parse(dr["created"].ToString()),
                    AssignedTo = dr["assigned_to"].ToString(),
                    ResourceID = Utility.ConvertTo(dr["resource_id"], 0)
                });
            }

            return result;
        }
    }
}
