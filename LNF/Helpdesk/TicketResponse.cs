using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Helpdesk
{
    public class TicketResponse
    {
        public int ResponseID { get; set; }
        public int MsgID { get; set; }
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string Response { get; set; }
        public string IPAddress {get;set;}
        public string Created {get;set;}
        public int Attachments { get; set; }
    }
}
