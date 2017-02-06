using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Control;

namespace LNF.Impl.Mappings.Control
{
    public class UnloadReservationMap : ClassMap<UnloadReservation>
    {
        public UnloadReservationMap()
        {
            Schema("sselControl.dbo");
            Table("v_UnloadReservation");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.UnloadTime);
            Map(x => x.Index, "[Index]");
            Map(x => x.Point);
            Map(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ActivityID);
            Map(x => x.ActivityName);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.NextReservationBeginDateTime);
        }
    }
}
