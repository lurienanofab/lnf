using LNF.Data;
using System;

namespace LNF.Impl.Scripting
{
    public class DefaultScriptEngine : IScriptEngine
    {
        public virtual ScriptResult Run(string script, ScriptParameters parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
