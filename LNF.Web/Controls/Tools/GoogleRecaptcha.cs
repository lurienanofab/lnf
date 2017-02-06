using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            using (var hc = new HttpClient())
            {
                var postData = new Dictionary<string, string>();
                postData.Add("secret", GetSecretKey());
                postData.Add("response", GetResponse());
                postData.Add("remoteip", GetRemoteIP());

                hc.BaseAddress = new Uri("https://www.google.com/");

                var msg = await hc.PostAsync("recaptcha/api/siteverify", new FormUrlEncodedContent(postData));

                var json = await msg.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<VerificationResponse>(json);

                return result;
            }
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
