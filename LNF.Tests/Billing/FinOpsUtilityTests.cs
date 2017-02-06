using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LNF;
using LNF.Web;
using LNF.Data;
using LNF.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.CommonTools;

namespace LNF.Tests.Billing
{
    [TestClass]
    public class FinOpsUtilityTests
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public Task CanUpdateAccountOnFinOps()
        {
            throw new NotImplementedException();
            //Reservation rsv = DA.Current.Single<Reservation>(566299);
            //rsv.Account = DA.Current.Single<Account>(143);
            //DA.Current.SaveOrUpdate(rsv);
            //DA.Current.Flush();
            //await FinOpsUtility.UpdateAccountOnFinOps(rsv);
        }
    }
}
