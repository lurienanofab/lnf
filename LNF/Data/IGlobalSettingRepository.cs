using System.Collections.Generic;

namespace LNF.Data
{
    public interface IGlobalSettingRepository
    {
        IEnumerable<IGlobalSetting> GetGlobalSettings();
        IGlobalSetting GetGlobalSetting(string name);
    }
}