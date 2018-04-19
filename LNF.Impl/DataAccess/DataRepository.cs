using LNF.Data;
using LNF.Repository.Data;
using NHibernate;
using System;
using System.Collections.Generic;

namespace LNF.Impl.DataAccess
{
    public class DataRepository : IDataRepository
    {
        protected ISession Session { get; }

        public DataRepository(ISession session)
        {
            Session = session;
        }

        public IEnumerable<Account> FindActiveAccountsInDateRange(int clientId, DateTime sd, DateTime ed)
        {
            Account acct = null;
            ClientAccount ca = null;
            ClientOrg co = null;
            ActiveLog alog1 = null;
            ActiveLog alog2 = null;

            if (!Session.IsOpen)
                throw new Exception("WTF?");

            var result = Session.QueryOver(() => acct)
                .JoinEntityAlias(() => ca, () => ca.Account.AccountID == acct.AccountID)
                .JoinEntityAlias(() => co, () => co.ClientOrgID == ca.ClientOrg.ClientOrgID)
                .JoinEntityAlias(() => alog1, () => alog1.Record == ca.ClientAccountID && alog1.TableName == "ClientAccount")
                .JoinEntityAlias(() => alog2, () => alog2.Record == ca.ClientOrg.ClientOrgID && alog2.TableName == "ClientOrg")
                .Where(() =>
                    co.Client.ClientID == clientId
                    && (alog1.EnableDate < ed && (alog1.DisableDate == null || alog1.DisableDate > ed))
                    && (alog2.EnableDate < ed && (alog2.DisableDate == null || alog2.DisableDate > ed)))
                .List();

            return result;
        }
    }
}
