using System;

namespace LNF.Web.Models
{
    public class SessionLogMessage
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public DateTime LogDateTime { get; set; }
        public string Text { get; set; }
        public string Message => $"[{LogDateTime:yyyy-MM-dd HH:mm:ss}][{ClientID}:{UserName}] {Text}";
        public override string ToString() => Message;
    }
}
