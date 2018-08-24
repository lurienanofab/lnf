using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        private IUnitOfWork _uow;

        [TestInitialize]
        public void TestInitialize()
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();
            _uow = ServiceProvider.Current.DataAccess.StartUnitOfWork();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _uow.Dispose();
        }
    }
}
