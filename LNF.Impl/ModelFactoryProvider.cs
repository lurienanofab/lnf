using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Impl.DataAccess.ModelFactory;
using System;

namespace LNF.Impl
{
    public class ModelFactoryProvider
    {
        public static ModelFactoryProvider Current { get; private set; }

        public static void Setup(IModelFactory factory)
        {
            var stack = new System.Diagnostics.StackTrace();

            if (Current == null)
            {
                //var factory = new ValueInjecterModelFactory(mgr);
                Current = new ModelFactoryProvider(factory);
                if (SessionLog.Enabled)
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Model setup complete.{Environment.NewLine}{stack}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Model has already been setup. What are you trying to do?{Environment.NewLine}{stack}");
            }
        }

        public IModelFactory Factory { get; }

        private ModelFactoryProvider(IModelFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }
    }
}
