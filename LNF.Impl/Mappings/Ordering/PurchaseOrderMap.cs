using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaseOrderMap : ClassMap<PurchaseOrder>
    {
        public PurchaseOrderMap()
        {
            Schema("IOF.dbo");
            Id(x => x.POID);
            References(x => x.Client);
            References(x => x.Vendor);
            Map(x => x.AccountID);
            References(x => x.Approver);
            Map(x => x.CreatedDate);
            Map(x => x.NeededDate);
            Map(x => x.Oversized);
            References(x => x.ShippingMethod);
            Map(x => x.Notes);
            References(x => x.Status);
            Map(x => x.CompletedDate);
            Map(x => x.RealApproverID);
            Map(x => x.ApprovalDate);
            Map(x => x.Attention);
            Map(x => x.PurchaserID);
            Map(x => x.RealPO, "RealPOID");
            Map(x => x.ReqNum);
            Map(x => x.PurchaserNotes);
        }
    }
}
