using System;
using System.Linq;

namespace LNF.Web
{
    public static class SessionKeys
    {
        public static readonly string CurrentUser = "CurrentUser";
        public static readonly string ClientID = "ClientID";
        public static readonly string UserName = "UserName";
        public static readonly string DisplayName = "DisplayName";
        public static readonly string Email = "Email";

        public static readonly string Cache = "Cache";
        public static readonly string ErrorID = "ErrorID";

        public static readonly string Date = "Date";
        public static readonly string ReservationID = "ReservationID";
        public static readonly string WeekStartDate = "WeekStartDate";
        public static readonly string ToolEngineers = "ToolEngineers";
        public static readonly string ClientSetting = "ClientSetting";
        public static readonly string DisplayDefaultHours = "DisplayDefaultHours";
        public static readonly string RemovedInvitees = "RemovedInvitees";

        public static string[] AllKeys()
        {
            return typeof(SessionKeys)
                .GetFields()
                .Select(x => x.GetValue(null).ToString())
                .ToArray();
        }
    }
}
