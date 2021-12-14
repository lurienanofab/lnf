using LNF.DataAccess;
using LNF.DependencyInjection;
using LNF.Impl.DependencyInjection;
using LNF.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LNF.Tests
{
    [TestClass]
    public class ReservationUtilityTests
    {
        private IProvider _provider;
        private IContainerContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = ContainerContextFactory.Current.NewThreadScopedContext();

            var cfg = new ThreadStaticContainerConfiguration(_context);
            cfg.RegisterAllTypes();
            
            _provider = _context.GetInstance<IProvider>();
            ServiceProvider.Setup(_provider);
        }

        [TestMethod]
        public void CanHandleUnstartedReservations()
        {
            using (StartUnitOfWork())
            {
                var items = ServiceProvider.Current.Scheduler.Reservation.SelectPastUnstarted().Where(x => x.ReservationID == 964556).ToList(); //new ReservationItem { ReservationID = 952692 };
                Assert.IsTrue(items.Count == 1);
                Reservations.Create(_provider, DateTime.Now).HandleUnstartedReservations(items);
            }
        }

        [TestMethod]
        public void CanSelectPastEndableRepair()
        {
            using (StartUnitOfWork())
            {
                var reservations = ServiceProvider.Current.Scheduler.Reservation.SelectPastUnstarted();
                Assert.IsTrue(reservations.Count() > 0);
            }
        }

        [TestMethod]
        public void CanHandleAutoEndReservations()
        {
            using (StartUnitOfWork())
            {
                var reservationId = 966238;
                var items = ServiceProvider.Current.Scheduler.Reservation.SelectAutoEnd().Where(x => x.ReservationID == reservationId).ToList();
                Assert.IsTrue(items.Count == 1);
                Reservations.Create(_provider, DateTime.Now).HandleAutoEndReservations(items);
            }
        }

        public IUnitOfWork StartUnitOfWork()
        {
            return _context.GetInstance<IUnitOfWork>();
        }
    }
}
