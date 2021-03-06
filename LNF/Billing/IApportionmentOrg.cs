﻿using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IApportionmentOrg
    {
        int OrgID { get; set; }
        string OrgName { get; set; }
        double MinimumDays { get; set; }
        IEnumerable<IApportionmentAccount> Accounts { get; set; }
    }
}
