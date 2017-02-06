﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.PhysicalAccess
{
    public class Badge
    {
        public object ID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime? CurrentAccessTime { get; set; }
        public int? CurrentCardNumber { get; set; }
        public string CurrentEventDescription { get; set; }
        public string CurrentAreaName { get; set; }
        public string AltDescription { get; set; }

        public IEnumerable<Card> GetCards()
        {
            return Providers.PhysicalAccess.GetCards(this);
        }
    }
}
