using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Hooks
{

    public abstract class Hook<TContext, TResult> : IHook<TContext, TResult>
        where TContext : HookContext
        where TResult : HookResult
    {
        private TContext _Context;
        private TResult _Result;

        public TContext Context
        {
            get { return _Context; }
        }

        public TResult Result
        {
            get { return _Result; }
        }

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

        internal Hook() { }

        public void Execute(TContext context, TResult result)
        {
            _Context = context;
            _Result = result;
            Execute();
        }

        abstract protected void Execute();
    }
}
