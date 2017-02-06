using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using NHibernate;

namespace LNF.Impl.Testing
{
    //[TestClass]
    public abstract class NHibernateTestBase
    {
        public DateTime StartTime { get; set; }
        public long StartingMemory { get; set; }

        //[TestInitialize]
        public virtual void TestSetup()
        {
            StartTime = DateTime.Now;
            StartingMemory = System.GC.GetTotalMemory(false);
            Prepare();
        }

        //Sample implementation of GetContext using Rhino
        /*
        protected override IContext GetContext()
        {
            ISession session = null;
            IContext context = MockRepository.GenerateStub<IContext>();
            context.Stub(x => x.SetItem("nhibernate.current_session", session));
            context.Stub(x => x.GetItem("nhibernate.current_session")).Return(session);
            return context;
        }
        */

        protected abstract void Prepare();

        protected virtual void ResetTimer(string message = null)
        {
            TimeSpan timer = DateTime.Now - StartTime;
            long memoryDelta = System.GC.GetTotalMemory(false) - StartingMemory;

            StartTime = DateTime.Now;
            StartingMemory = System.GC.GetTotalMemory(false);

            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);
            Console.WriteLine("Total Seconds: " + timer.TotalSeconds.ToString());
            Console.WriteLine("Memory Delta (KB): " + (memoryDelta / 1024).ToString());
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
