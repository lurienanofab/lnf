using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Hooks
{

    public abstract class Hook<Tcontext, Tresult> : IHook<Tcontext, Tresult>
        where Tcontext : HookContext
        where Tresult : HookResult
    {
        private Tcontext _Context;
        private Tresult _Result;

        public Tcontext Context
        {
            get { return _Context; }
        }

        public Tresult Result
        {
            get { return _Result; }
        }

        public virtual int Priority
        {
            get { return 0; }
        }

        public Type GetContextType()
        {
            return typeof(Tcontext);
        }

        public Type GetResultType()
        {
            return typeof(Tresult);
        }

        internal Hook() { }

        public void Execute(Tcontext context, Tresult result)
        {
            _Context = context;
            _Result = result;
            Execute();
        }

        abstract protected void Execute();
    }
}
