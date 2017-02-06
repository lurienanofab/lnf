using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Hooks
{
    public interface IHook<Tcontext, Tresult> : IHook
        where Tcontext : HookContext
        where Tresult : HookResult
    {
        Tcontext Context { get; }
        Tresult Result { get; }
        void Execute(Tcontext context, Tresult result);
    }

    public interface IHook
    {
        Type GetContextType();
        Type GetResultType();
        int Priority { get; }
    }
}
