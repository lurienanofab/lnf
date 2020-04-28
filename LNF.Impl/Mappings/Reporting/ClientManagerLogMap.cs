using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Reporting;

namespace LNF.Impl.Mappings.Reporting
{
    internal class ClientManagerLogMap : ClassMap<ClientManagerLog>
    {
        internal ClientManagerLogMap()
        {
            Schema("Reporting.dbo");
            Table("v_ClientManagerLog");
            ReadOnly();

            CompositeId()
                .KeyProperty(x => x.ManagerLogID)
                .KeyProperty(x => x.UserLogID);
            Map(x => x.ManagerTableName);
            Map(x => x.ManagerRecord);
            Map(x => x.ManagerEnableDate);
            Map(x => x.ManagerDisableDate);
            Map(x => x.UserTableName);
            Map(x => x.UserRecord);
            Map(x => x.UserEnableDate);
            Map(x => x.UserDisableDate);
            Map(x => x.ManagerClientID);
            Map(x => x.ManagerUserName);
            Map(x => x.ManagerLName);
            Map(x => x.ManagerFName);
            Map(x => x.ManagerEmail);
            Map(x => x.UserClientID);
            Map(x => x.UserUserName);
            Map(x => x.UserLName);
            Map(x => x.UserFName);
            Map(x => x.UserEmail);
            Map(x => x.ManagerIsTechnicalManager);
            Map(x => x.ManagerIsFinancialManager);
            Map(x => x.UserIsTechnicalManager);
            Map(x => x.UserIsFinancialManager);
            Map(x => x.UserManager);
            Map(x => x.UserPrivs);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
            Map(x => x.AccountNumber);
            Map(x => x.OrgID);
            Map(x => x.OrgName);
            Map(x => x.IsSubsidyOrg);
            Map(x => x.IsRemote);
            Map(x => x.RemoteClientClientID);
            Map(x => x.RemoteClientUserName);
            Map(x => x.RemoteClientLName);
            Map(x => x.RemoteClientFName);
            Map(x => x.RemoteClientEmail);
        }
    }
}
