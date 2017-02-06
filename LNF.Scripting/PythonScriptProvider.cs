using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF;

namespace LNF.Scripting
{
    public class PythonScriptProvider : IScriptingProvider
    {
        public Result Run(string script, Parameters parameters = null)
        {
            if (string.IsNullOrEmpty(script))
                throw new Exception("No script was provided");

            Engine eng = ScriptingContext.Engine;
            
            eng.Run(script, parameters);
          
            return eng.Result;
        }

        public void Dispose()
        {

        }
    }
}
