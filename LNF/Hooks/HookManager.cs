using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace LNF.Hooks
{
    public enum HookTypes
    {
        BeforeLogin = 1,
        AfterLogin = 0
    }

    public static class HookManager
    {
        private readonly static List<Type> _hooks;

        static HookManager()
        {
            _hooks = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                try
                {
                    Type[] types = a.GetTypes();
                    foreach (Type t in types)
                    {
                        if (typeof(IHook).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        {
                            _hooks.Add(t);
                        }
                    }
                }
                catch { }
            }
        }

        public static void RunHooks<Tcontext, Tresult>(Tcontext context, Expression<Action<Tcontext, Tresult>> handler = null)
            where Tcontext : HookContext
            where Tresult: HookResult, new()
        {
            HookManager.RunHooks(context, new Tresult(), handler);
        }

        public static void RunHooks<Tcontext, Tresult>(Tcontext context, Tresult result, Expression<Action<Tcontext, Tresult>> handler = null)
            where Tcontext : HookContext
            where Tresult : HookResult
        {
           
            List<IHook<Tcontext, Tresult>> hooks = _hooks
                .Where(x => typeof(IHook<Tcontext, Tresult>).IsAssignableFrom(x))
                .Select(x => (IHook<Tcontext, Tresult>)Activator.CreateInstance(x))
                .ToList();

            foreach (IHook<Tcontext, Tresult> h in hooks.OrderByDescending(x => x.Priority))
            {
                h.Execute(context, result);
            }
            if (handler != null)
                handler.Compile().Invoke(context, result);
        }

        public static string[] GetHookTypes()
        {
            return _hooks.Select(x => x.ToString()).ToArray();
        }
    }
}
