using LNF.DataAccess;
using LNF.DependencyInjection;
using LNF.Impl.DataAccess;
using LNF.Impl.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        private IContainerContext _context;
        private IDisposable _uow;

        public ISessionManager SessionManager => _context.GetInstance<ISessionManager>();
        public NHibernate.ISession Session => SessionManager.Session;
        public IProvider Provider => _context.GetInstance<IProvider>();

        [TestInitialize]
        public void TestInitialize()
        {
            _context = ContainerContextFactory.Current.NewThreadScopedContext();

            var cfg = new ThreadStaticContainerConfiguration(_context);
            cfg.RegisterAllTypes();

            ServiceProvider.Setup(Provider);
            _uow = StartUnitOfWork();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _uow.Dispose();
        }

        public void AssertObjectsAreSame(object obj1, object obj2)
        {
            if (obj1 == null) throw new ArgumentNullException("obj1");
            if (obj2 == null) throw new ArgumentNullException("obj2");

            var t1 = obj1.GetType();
            var t2 = obj2.GetType();

            var props1 = obj1.GetType().GetProperties();

            foreach (var p1 in props1)
            {
                var v1 = p1.GetValue(obj1);

                var p2 = t2.GetProperty(p1.Name) ?? throw new Exception($"Property not found: The type {t2} does not contain property {p1.Name}.");

                var v2 = p2.GetValue(obj2);

                if (!p1.PropertyType.Equals(p2.PropertyType))
                    throw new Exception($"Types are not equal: {t1.Name}.{p1.Name} is type {p1.PropertyType} but {t2.Name}.{p2.Name} is type {p2.PropertyType}");

                if (v1 == null && v2 != null)
                    throw new Exception($"Values are not equal: {p1.Name} value is null but {p2.Name} value is {v2}");

                if (v1 != null && v2 == null)
                    throw new Exception($"Values are not equal: {p1.Name} value is {v1} but {p2.Name} value is null");

                if (v1 != null && !v1.Equals(v2))
                    throw new Exception($"Values are not equal: {p1.Name} value is {v1} but {p2.Name} value is {v2}");
            }
        }

        protected void AssertCollectionsAreSame<T1, T2>(IEnumerable<T1> c1, IEnumerable<T2> c2, Func<T1, T2, bool> getter, Comparer<T1, T2> comparer)
        {
            var list1 = c1.ToList();
            var list2 = c2.ToList();

            Assert.AreEqual(list1.Count, list2.Count);

            foreach (var item1 in list1)
            {
                var item2 = list2.First(i2 => getter(item1, i2));
                Assert.IsTrue(comparer.AreEqual(item1, item2));
            }
        }

        protected SqlConnection NewConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        }

        public IUnitOfWork StartUnitOfWork() => Provider.DataAccess.StartUnitOfWork();
    }

    public abstract class Comparer<T1, T2>
    {
        public abstract bool AreEqual(T1 obj1, T2 obj2);
    }
}
