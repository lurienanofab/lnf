using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Scripting
{
    public static class ScriptingContext
    {
        private static Engine engine;

        static ScriptingContext()
        {
            engine = new Engine();
        }

        public static Engine Engine
        {
            get
            {
                if (engine == null)
                    engine = new Engine();
                return engine;
            }
        }

        public static void Dispose()
        {
            engine = null;
        }
    }
}
