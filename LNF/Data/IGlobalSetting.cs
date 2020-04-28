namespace LNF.Data
{
    public interface IGlobalSetting
    {
        int SettingID { get; set; }
        string SettingName { get; set; }
        string SettingValue { get; set; }
    }
}