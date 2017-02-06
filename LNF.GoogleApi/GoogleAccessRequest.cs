using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LNF.GoogleApi
{
    public class GoogleAccessRequest
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string[] Scopes { get; set; }
        public AccessType AccessType { get; set; }
        public string State { get; set; }
        public bool ForceApproval { get; set; }

        public string GetRequestUri()
        {
            string result = string.Format("https://accounts.google.com/o/oauth2/auth?scope={0}&redirect_uri={1}&response_type=code&client_id={2}&access_type={3}&approval_prompt={4}&state={5}",
                HttpUtility.UrlEncode(string.Join(" ", Scopes)),
                HttpUtility.UrlEncode(RedirectUri),
                HttpUtility.UrlEncode(ClientId),
                HttpUtility.UrlEncode(Enum.GetName(typeof(AccessType), AccessType).ToLower()),
                HttpUtility.UrlEncode(ForceApproval ? "force" : "auto"),
                HttpUtility.UrlEncode(State));

            return result;
        }
    }

    public enum AccessType
    {
        Online = 1,
        Offline = 2
    }
}
