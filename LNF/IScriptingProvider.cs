using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Scripting;

namespace LNF
{
    public interface IScriptingProvider : ITypeProvider
    {
        Result Run(string script, Parameters parameters = null);
    }
}
