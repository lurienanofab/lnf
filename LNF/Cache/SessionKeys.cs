using System.Linq;

namespace LNF.Cache
{
    public static class SessionKeys
    {
        public static readonly string CurrentUser = "CurrentUser";

        public static readonly string Cache = "Cache";
        public static readonly string IsKiosk = "IsKiosk";
        public static readonly string IsOnKiosk = "IsOnKiosk";
        public static readonly string ClientLabs = "ClientLabs";
        public static readonly string ErrorID = "ErrorID";

        public static readonly string Date = "Date";
        public static readonly string ReservationID = "ReservationID";
        public static readonly string WeekStartDate = "WeekStartDate";
        public static readonly string ToolEngineers = "ToolEngineers";
        public static readonly string ClientSetting = "ClientSetting";
        public static readonly string DisplayDefaultHours = "DisplayDefaultHours";
        public static readonly string ReservationProcessInfos = "ReservationProcessInfos";
        public static readonly string ReservationInvitees = "ReservationInvitees";
        public static readonly string AvailableInvitees = "AvailableInvitees";
        public static readonly string RemovedInvitees = "RemovedInvitees";

        public static string[] AllKeys()
        {
            return typeof(SessionKeys)
                .GetFields()
                .Select(x => x.GetValue(null).ToString())
                .ToArray();
        }

        public static void RemoveAll()
        {
            foreach (string key in AllKeys())
                ServiceProvider.Current.Context.RemoveSessionValue(key);
        }
    }
}
