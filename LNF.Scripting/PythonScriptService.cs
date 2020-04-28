using LNF.Data;
using System;

namespace LNF.Scripting
{
    public class PythonScriptService : IScriptEngine
    {
        public ScriptResult Run(string script, ScriptParameters parameters = null)
        {
            if (string.IsNullOrEmpty(script))
                throw new Exception("No script was provided");

            Engine eng = ScriptingContext.Engine;
            
            eng.Run(script, parameters);
          
            return eng.Result;
        }
    }
}
