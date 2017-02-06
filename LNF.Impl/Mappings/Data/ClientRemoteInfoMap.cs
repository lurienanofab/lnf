using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ClientRemoteInfoMap : ClassMap<ClientRemoteInfo>
    {
        public ClientRemoteInfoMap()
        {
            Schema("sselData.dbo");
            Table("v_ClientRemoteInfo");
            ReadOnly();
            Id(x => x.ClientRemoteID);
            Map(x => x.ClientID);
            Map(x => x.UserName);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.MName);
            Map(x => x.DisplayName);
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.Email);
            Map(x => x.Phone);
            Map(x => x.ClientActive);
            Map(x => x.RemoteClientID);
            Map(x => x.RemoteUserName);
            Map(x => x.RemoteLName);
            Map(x => x.RemoteFName);
            Map(x => x.RemoteMName);
            Map(x => x.RemoteDisplayName);
            Map(x => x.RemotePrivs);
            Map(x => x.RemoteCommunities);
            Map(x => x.RemoteEmail);
            Map(x => x.RemotePhone);
            Map(x => x.RemoteClientActive);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.Number);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.OrgActive);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.AccountTypeID);
            Map(x => x.AccountTypeName);
            Map(x => x.AccountActive);
        }
    }
}
