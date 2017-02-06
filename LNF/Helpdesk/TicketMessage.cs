using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Helpdesk
{
    public class TicketMessage
    {
        public int MsgID { get; set; }
        public string Created { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string IPAddress { get; set; }
        public int Attachments { get; set; }
    }
}
