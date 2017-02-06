using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:MessageBox runat=server></{0}:MessageBox>")]
    public class MessageBox : WebControl
    {
        private string _Content;

        public void Alert(string message)
        {
            _Content = "<script type=\"text/javascript\">alert('" + ServerJScript.JSEncode(message) + "');</script>";
        }

        public void Confirm(string message)
        {
            _Content = "<input type=\"hidden\" value=\"0\" name=\"" + ID + "_result\" />" + Environment.NewLine
                + "<script type=\"text/javascript\">" + Environment.NewLine
                + "  if (confirm('" + ServerJScript.JSEncode(message) + "')) {" + Environment.NewLine
                + "    document.forms[0]." + ID + "_result.value='" + WebMessageBoxResult.Ok + "'; }" + Environment.NewLine
                + "  else {" + Environment.NewLine
                + "    document.forms[0]." + ID + "_result.value='" + WebMessageBoxResult.Cancel + "'; }" + Environment.NewLine
                + "  document.forms[0].submit(); " + Environment.NewLine
                + "</script>";
        }

        public event EventHandler<WebMessageBoxEventArg> MessageBoxOK;

        protected virtual void OnMessageBoxOK(WebMessageBoxEventArg e)
        {
            if (MessageBoxOK != null)
                MessageBoxOK(this, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (HttpContext.Current.Request.Form[ID + "_result"] == null)
                return;

            if ((WebMessageBoxResult)Enum.Parse(typeof(WebMessageBoxResult), HttpContext.Current.Request.Form[ID + "_result"]) == WebMessageBoxResult.Ok)
                OnMessageBoxOK(new WebMessageBoxEventArg(WebMessageBoxResult.Ok));
            else if ((WebMessageBoxResult)Enum.Parse(typeof(WebMessageBoxResult), HttpContext.Current.Request.Form[ID + "_result"]) == WebMessageBoxResult.Cancel)
                OnMessageBoxOK(new WebMessageBoxEventArg(WebMessageBoxResult.Cancel));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered("MessageBox_" + ID) && !string.IsNullOrEmpty(_Content))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "MessageBox_" + ID, _Content);
                _Content = null;
            }
        }
    }

    public enum WebMessageBoxResult
    {
        Cancel = 0,
        Ok = 1
    }

    public class WebMessageBoxEventArg : EventArgs
    {
        private WebMessageBoxResult _Result;

        public WebMessageBoxResult MessageResult
        {
            get { return _Result; }
        }

        public WebMessageBoxEventArg(WebMessageBoxResult result)
        {
            _Result = result;
        }
    }
}