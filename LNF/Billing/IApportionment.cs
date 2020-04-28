using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IApportionment
    {
        int ClientID { get; set; }
        int RoomID { get; set; }
        DateTime Period { get; set; }
        double TotalDaysInLab { get; set; }
        IEnumerable<IApportionmentOrg> Orgs { get; set; }
    }
}
