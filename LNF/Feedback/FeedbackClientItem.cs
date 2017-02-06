using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.PhysicalAccess;
using LNF.Repository.Data;

namespace LNF.Feedback
{
    public class FeedbackClientItem
    {
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
    }
}
