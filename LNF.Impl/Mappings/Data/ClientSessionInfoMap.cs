﻿using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ClientSessionInfoMap : ClassMap<ClientSessionInfo>
    {
        internal ClientSessionInfoMap()
        {
            Schema("sselData.dbo");
            ReadOnly();
            Id(x => x.ClientID);
            Map(x => x.MName);
            Map(x => x.LName);
            Map(x => x.UserName);
            Map(x => x.Password);
            Map(x => x.PasswordHash);
            Map(x => x.DemCitizenID);
            Map(x => x.DemGenderID);
            Map(x => x.DemRaceID);
            Map(x => x.DemEthnicID);
            Map(x => x.DemDisabilityID);
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.TechnicalInterestID);
            Map(x => x.Active);
            Map(x => x.IsChecked, "isChecked");
            Map(x => x.IsSafetyTest, "isSafetyTest");
            Map(x => x.OrgID);
            Map(x => x.Email);
            Map(x => x.Phone);
            Map(x => x.MaxChargeTypeID);
            Map(x => x.DisplayName);
        }
    }
}
