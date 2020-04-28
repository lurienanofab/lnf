using LNF.CommonTools;

namespace LNF.Scripting
{
    public class Tools
    {
        public WriteData WriteData { get; }

        public string GetVersion()
        {
            return CommonTools.Utility.Version();
        }

        public Tools()
        {
            WriteData = new WriteData(ServiceProvider.Current);
        }
    }
}
