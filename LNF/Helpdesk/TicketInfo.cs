using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Helpdesk
{
    public class TicketInfo
    {
        public int TicketID { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string DeptName { get; set; }
        public string Created { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Source { get; set; }
        public string AssignedName { get; set; }
        public string AssignedEmail { get; set; }
        public string HelpTopic { get; set; }
        public string LastResponse { get; set; }
        public string LastMessage { get; set; }
        public string IPAddress { get; set; }
        public string DueDate { get; set; }
    }
}
