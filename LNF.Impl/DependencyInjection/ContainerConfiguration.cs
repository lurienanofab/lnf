using LNF.DependencyInjection;
using System;

namespace LNF.Impl.DependencyInjection
{
    public abstract class ContainerConfiguration
    {
        public IContainerContext Context { get; }

        public bool SkipDataAccessRegistration { get; set; }

        public ContainerConfiguration(IContainerContext context)
        {
            Context = context ?? throw new ArgumentNullException("context");
            if (context.IsLocked()) throw new ArgumentException("The container cannot already be locked.");
        }

        public abstract void RegisterContext();

        public abstract void RegisterSessionManager();

        public abstract void RegisterDataAccessService();
    }

}
