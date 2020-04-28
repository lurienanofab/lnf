using System;

namespace LNF.Hooks
{

    public abstract class Hook<TContext, TResult> : IHook<TContext, TResult>
        where TContext : HookContext
        where TResult : HookResult
    {
        public IProvider Provider { get; }

        public TContext Context { get; private set; }

        public TResult Result { get; private set; }

        public virtual int Priority
        {
            get { return 0; }
        }

        public Type GetContextType()
        {
            return typeof(TContext);
        }

        public Type GetResultType()
        {
            return typeof(TResult);
        }

        internal Hook(IProvider provider)
        {
            Provider = provider;
        }

        public void Execute(TContext context, TResult result)
        {
            Context = context;
            Result = result;
            Execute();
        }

        abstract protected void Execute();
    }
}
