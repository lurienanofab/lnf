using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Tools
{
    [ToolboxData("<{0}:ScrollPositionTool runat=server></{0}:ScrollPositionTool>")]
    public class ScrollPositionTool : WebControl
    {
        private string _Target = "$(window)";

        public string Target
        {
            get
            {
                return _Target;
            }
            set
            {
                string temp = (value == string.Empty) ? "$(window)" : value;
                _Target = temp;
            }
        }

        public int CurrentScrollPosition { get; set; }

        public string HiddenFieldID
        {
            get
            {
                this.EnsureID();
                return this.ID + "_CurrentScrollPosition";
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            HiddenField hid = new HiddenField();
            hid.ID = this.HiddenFieldID;
            if (!Page.IsPostBack) hid.Value = this.CurrentScrollPosition.ToString();
            this.Controls.Add(hid);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HiddenField hid = (HiddenField)this.FindControl(this.HiddenFieldID);
            this.CurrentScrollPosition = Convert.ToInt32(hid.Value);
            LiteralControl lit = new LiteralControl();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine(_Target + ".scroll(function(e){");
            sb.AppendLine("  $('#" + hid.ClientID + "').val(" + _Target + ".scrollTop());");
            sb.AppendLine("});");
            sb.AppendLine("$(document).ready(function(){");
            sb.AppendLine("  " + _Target + ".scrollTop(" + hid.Value + ");");
            sb.AppendLine("});");
            sb.AppendLine("</script>");
            lit.Text = sb.ToString();
            this.Controls.Add(lit);
        }
    }
}
