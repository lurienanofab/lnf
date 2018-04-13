namespace LNF.CommonTools
{
    public static class SendEmail
    {
        public static void Email(string fromAddr, string toAddr, bool ccSelf, string subject, string body, bool isHtml = true)
        {
            string[] to = new string[] { toAddr };
            string[] cc = (ccSelf) ? new string[] { fromAddr } : null;
            ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.SendEmail.Email(string fromAddr, string toAddr, bool ccSelf, string subject, string body, bool isHtml = true)", subject, body, fromAddr, to, cc, isHtml: isHtml);
        }

        public static string SystemEmail
        {
            get
            {
                return "lnf-system@umich.edu"; //should really come from DB
            }
        }

        public static string[] DeveloperEmails
        {
            get
            {
                //var result = Client.SelectByPriv(ClientPrivilege.Developer).Select(x => x.PrimaryEmail()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return new[] { "lnf-it@umich.edu" };
            }
        }
    }
}
