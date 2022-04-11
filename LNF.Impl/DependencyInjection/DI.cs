using LNF.DependencyInjection;
using SimpleInjector;
using System;

namespace LNF.Impl.DependencyInjection
{
    public static class DI
    {
        private static ThreadScopeProvider _threadScope = null;
        private static AsyncScopeProvider _asyncScope = null;
        private static WebScopeProvider _webScope = null;

        public static ThreadScopeProvider ThreadScope
        {
            get
            {
                if (_threadScope == null)
                    _threadScope = new ThreadScopeProvider();
                return _threadScope;
            }
        }

        public static AsyncScopeProvider AsyncScope
        {
            get
            {
                if (_asyncScope == null)
                    _asyncScope = new AsyncScopeProvider();
                return _asyncScope;
            }
        }

        public static WebScopeProvider WebScope
        {
            get
            {
                if (_webScope == null)
                    _webScope = new WebScopeProvider();
                return _webScope;
            }
        }

        public static void EnsureContextExists()
        {
            if (!ContainerContextFactory.Current.ContextExists())
                throw new Exception("Context does not exist. Call Initialize first.");
        }
    }


    public abstract class ScopeProvider
    {
        protected SimpleInjectorContainerContext _context;

        public ScopeProvider()
        {
            InitializeContext();
            ServiceProvider.Setup(GetContext().GetInstance<IProvider>());
        }

        protected abstract void InitializeContext();
        public SimpleInjectorContainerContext GetContext() => ContainerContextFactory.Current.GetContext();
        public IProvider GetProvider() => GetContext().GetInstance<IProvider>();
    }

    public sealed class ThreadScopeProvider : ScopeProvider
    {
        protected override void InitializeContext()
        {
            ContainerContextFactory.Current.NewThreadScopedContext();
            _context = ContainerContextFactory.Current.GetContext();
            var cfg = new ThreadStaticContainerConfiguration(_context);
            cfg.RegisterAllTypes();
        }
    }

    public sealed class AsyncScopeProvider : ScopeProvider
    {
        protected override void InitializeContext()
        {
            ContainerContextFactory.Current.NewAsyncScopedContext();
            _context = ContainerContextFactory.Current.GetContext();
            var cfg = new WebContainerConfiguration(_context);
            cfg.RegisterAllTypes();
        }
    }

    public sealed class WebScopeProvider : ScopeProvider
    {
        protected override void InitializeContext()
        {
            ContainerContextFactory.Current.NewWebRequestContext();
            _context = ContainerContextFactory.Current.GetContext();
            var cfg = new WebContainerConfiguration(_context);
            cfg.RegisterAllTypes();
        }
    }
}
