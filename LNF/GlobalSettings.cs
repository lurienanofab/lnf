using LNF.Repository;
using System;
using System.Data;

namespace LNF
{
    public class GlobalSettings
    {
        public static GlobalSettings Current { get; }

        private DataTable _table;

        private GlobalSettings() 
        {
            LoadTable();
        }

        static GlobalSettings()
        {
            Current = new GlobalSettings();
        }

        public string CompanyName => GetRequiredGlobalSetting("CompanyName");
        public string SystemEmail => GetRequiredGlobalSetting("SystemEmail");
        public string[] DeveoperEmails => GetRequiredGlobalSetting("DeveloperEmails").Split(',');
        public string DebugEmail => GetRequiredGlobalSetting("DebugEmail");
        public bool UseParentRooms => bool.Parse(GetRequiredGlobalSetting("UseParentRooms"));
        public string FinancialManagerUserName => GetRequiredGlobalSetting("FinancialManagerUserName");

        private void LoadTable()
        {
            string sql = "SELECT * FROM sselData.dbo.GlobalSettings";
            _table = DataCommand.Create(CommandType.Text).FillDataTable(sql);
        }

        public string GetGlobalSetting(string name)
        {
            var rows = _table.Select($"SettingName = '{name}'");
            if (rows.Length == 0) return null;
            var result = rows[0].Field<string>("SettingValue");
            return result;
        }

        private string GetRequiredGlobalSetting(string name)
        {
            var result = GetGlobalSetting(name);

            if (string.IsNullOrEmpty(result))
                throw new Exception($"Missing required GlobalSetting: {name}");

            return result;
        }
    }
}
