using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Helpdesk
{
    public class TicketDetailResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public TicketDetail Detail { get; set; }
    }
}
