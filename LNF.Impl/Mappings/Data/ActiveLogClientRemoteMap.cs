using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogClientRemoteMap : ClassMap<ActiveLogClientRemote>
    {
        internal ActiveLogClientRemoteMap()
        {
            Schema("sselData.dbo");
            Table("v_ActiveLogClientRemote");
            ReadOnly();

            Id(x => x.LogID);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
            Map(x => x.ClientRemoteID);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.RemoteClientID);
            Map(x => x.ClientLName);
            Map(x => x.ClientFName);
            Map(x => x.RemoteLName);
            Map(x => x.RemoteFName);
            Map(x => x.AccountName, "Name");
            Map(x => x.AccountNumber, "Number");
            Map(x => x.ShortCode);
            Map(x => x.OrgName);
        }
    }
}
