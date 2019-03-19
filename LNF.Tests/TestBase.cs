using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LNF.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        private IDisposable _uow;

        [TestInitialize]
        public void TestInitialize()
        {
            var ioc = new IOC(new ContextFactory());
            ServiceProvider.Current = ioc.Resolver.GetInstance<ServiceProvider>();
            _uow = DA.StartUnitOfWork();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _uow.Dispose();
        }
    }
}
