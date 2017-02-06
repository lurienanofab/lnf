using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Helpdesk
{
    public class TicketDetail
    {
        public TicketInfo Info { get; set; }
        public List<TicketMessage> Messages { get; set; }
        public List<TicketResponse> Responses { get; set; }
    }
}
