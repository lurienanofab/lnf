using LNF.Scripting;

namespace LNF
{
    public interface IScriptingService
    {
        Result Run(string script, Parameters parameters = null);
    }
}
