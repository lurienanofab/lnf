using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Tools
{
    [ToolboxData("<{0}:GoogleRecaptcha runat=server></{0}:GoogleRecaptcha>")]
    public class GoogleRecaptcha : WebControl
    {
        public string GetSecretKey()
        {
            return ConfigurationManager.AppSettings["CaptchaPrivateKey"];
        }

        public string GetPublicKey()
        {
            return ConfigurationManager.AppSettings["CaptchaPublicKey"];
        }

        public string GetResponse()
        {
            return Page.Request.Form["g-recaptcha-response"];
        }

        protected override void CreateChildControls()
        {
            var div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "g-recaptcha");
            div.Attributes.Add("data-sitekey", GetPublicKey());

            this.Controls.Add(div);
        }

        //http://stackoverflow.com/questions/735350/how-to-get-a-users-client-ip-address-in-asp-net#740431
        public string GetRemoteIP()
        {
            string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        //https://developers.google.com/recaptcha/docs/verify
        public async Task<VerificationResponse> Verify()
        {
            var client = new RestClient("https://www.google.com");
            var request = new RestRequest("recaptcha/api/siteverify");
            request.AddParameter("secret", GetSecretKey());
            request.AddParameter("response", GetResponse());
            request.AddParameter("remoteip", GetRemoteIP());
            var response = await client.ExecutePostTaskAsync<VerificationResponse>(request);
            return response.Data;
        }
    }

    public class VerificationResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrrorCodes { get; set; }
    }
}
