namespace LNF.Data
{
    public interface IScriptEngine
    {
        ScriptResult Run(string script, ScriptParameters parameters = null);
    }
}
