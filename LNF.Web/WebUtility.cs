using LNF.CommonTools;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LNF.Web
{
    public static class WebUtility
    {
        public static void BindYearData(DropDownList ddl, int startYear = 2003, int endYear = 0)
        {
            if (endYear == 0) endYear = DateTime.Now.Year;
            ddl.DataSource = LNF.CommonTools.Utility.GetYears(startYear, endYear).Select(x => new { Year = x });
            ddl.DataValueField = "Year";
            ddl.DataTextField = "Year";
            ddl.DataBind();
        }

        public static string DataRowFilter(DataRow dr)
        {
            return string.Format("Period = '{0}' AND ClientID = {1} AND AccountID = {2}", dr["Period"], dr["ClientID"], dr["AccountID"]);
        }

        public static void FillRadioButtonList(RadioButtonList rbl, object inVal)
        {
            if (inVal == DBNull.Value)
                rbl.ClearSelection();
            else
                rbl.SelectedValue = inVal.ToString();
        }

        public static void FillDropDownLIst(DropDownList ddl, object inVal)
        {
            if (inVal == DBNull.Value)
                ddl.ClearSelection();
            else
                ddl.SelectedValue = inVal.ToString();
        }

        public static void FillCheckBox(CheckBox cb, object inVal)
        {
            if (inVal == DBNull.Value)
                cb.Checked = false;
            else
                cb.Checked = true;
        }

        public static string FillField(object inVal, string defVal)
        {
            if (inVal == DBNull.Value)
                return defVal;
            else
                return inVal.ToString();
        }

        public static string FillField<T>(object value, T defval, string format = "{0}")
        {
            T item;

            if (value == DBNull.Value)
                item = defval;
            else
                item = (T)value;

            string result = string.Format(format, item);

            return result;
        }

        public static string StripHTML(string html)
        {
            string result = string.Empty;
            Regex regExp = new Regex("<(.|\n)+?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            //Replace all HTML tag matches with the empty string
            result = regExp.Replace(html, "");

            return result;
        }

        public static System.Web.UI.Control FindControlRecursive(System.Web.UI.Control ctrl, string id)
        {
            if (ctrl.ID == id) return ctrl;

            foreach (System.Web.UI.Control ctl in ctrl.Controls)
            {
                System.Web.UI.Control found = FindControlRecursive(ctl, id);
                if (found != null) return found;
            }

            return null;
        }

        public static void BootstrapModal(System.Web.UI.Control control, string title, string text, string cssClass = null)
        {
            if (cssClass == null) cssClass = "";

            var modal = new HtmlGenericControl("div");
            modal.Attributes.Add("class", ("modal fade " + cssClass).Trim());
            modal.Attributes.Add("tabindex", "-1");
            modal.Attributes.Add("role", "dialog");

            var dialog = new HtmlGenericControl("div");
            dialog.Attributes.Add("class", "modal-dialog");

            var content = new HtmlGenericControl("div");
            content.Attributes.Add("class", "modal-content");

            var header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "modal-header");

            var button = new HtmlButton();
            button.Attributes.Add("class", "close");
            button.Attributes.Add("data-dismiss", "modal");
            button.Attributes.Add("aria-label", "Close");

            var span = new HtmlGenericControl("span");
            span.Attributes.Add("aria-hidden", "true");
            span.InnerHtml = "&times;";

            button.Controls.Add(span);

            var modalTitle = new HtmlGenericControl("h4");
            modalTitle.Attributes.Add("class", "modal-title");
            modalTitle.InnerHtml = title;

            header.Controls.Add(button);
            header.Controls.Add(modalTitle);

            var body = new HtmlGenericControl("div");
            body.Attributes.Add("class", "modal-body");

            var p = new HtmlGenericControl("p") { InnerHtml = text };
            body.Controls.Add(p);

            var footer = new HtmlGenericControl("div");
            footer.Attributes.Add("class", "modal-footer");

            button = new HtmlButton();
            button.Attributes.Add("class", "btn btn-primary");
            button.Attributes.Add("data-dismiss", "modal");
            button.InnerHtml = "Close";

            footer.Controls.Add(button);

            content.Controls.Add(header);
            content.Controls.Add(body);
            content.Controls.Add(footer);
            dialog.Controls.Add(content);
            modal.Controls.Add(dialog);

            control.Controls.AddAt(0, modal);
        }

        public static string BootstrapAlert(string type, string text, bool dismissible = false)
        {
            TagBuilder tag = new TagBuilder("div");
            tag.AddCssClass("alert");
            tag.AddCssClass("alert-" + type);
            tag.MergeAttribute("role", "alert");

            if (dismissible)
            {
                tag.AddCssClass("alert-dismissible");

                TagBuilder btn = new TagBuilder("button");
                btn.AddCssClass("close");
                btn.MergeAttributes(new Dictionary<string, string>()
                {
                    { "type", "button" },
                    { "data-dismiss", "alert" },
                    { "aria-label", "Close" }
                });

                TagBuilder span = new TagBuilder("span");
                span.MergeAttribute("aria-hidden", "true");
                span.InnerHtml = "&times;";

                btn.InnerHtml += span;
                tag.InnerHtml += btn;
            }

            tag.InnerHtml += text;

            return tag.ToString();
        }

        public static void BootstrapAlert(System.Web.UI.Control control, string type, string text, bool dismissible = false)
        {
            var alert = new LiteralControl(BootstrapAlert(type, text, dismissible));
            control.Controls.Add(alert);
        }

        public static void BootstrapTooltip(WebControl control, string text, string caption = null)
        {
            control.Attributes.Add("data-toggle", "tooltip");
            control.Attributes.Add("data-html", "true");
            control.Attributes.Add("data-placement", "right");
            control.Attributes.Add("data-animation", "false");
            control.Attributes.Add("data-container", "body");

            text = "<div class=\"tooltip-text\">" + text + "</div>";
            if (!string.IsNullOrEmpty(caption))
                text = "<div class=\"tooltip-caption\">" + caption + "</div>" + text;

            control.Attributes.Add("title", text);

            //string strMouseover = string.Empty;
            //if (cancelBubble)
            //    strMouseover = "event.cancelBubble=true;";
            //strMouseover += "return overlib('" + JSEncode(Tooltip) + "'";
            //if (!string.IsNullOrEmpty(Caption))
            //    strMouseover += ", CAPTION, '" + JSEncode(Caption) + "'";
            //strMouseover += ");";
            //Control.Attributes.Add("onmouseover", strMouseover);
            //Control.Attributes.Add("onmouseout", "return nd();");
        }

        /// <summary>
        /// Gets the site menu html for a particular user from a web service.
        /// </summary>
        public static IHtmlString GetSiteMenu(int clientId, string target)
        {
            // Why get the html from an ajax service?
            //  Because then there is one place where the menu is generated and can be used in different projects
            //  without having to copy and paste a partial view cshtml file resulting in a bunch of copies that
            //  can't be easily changed globally, or deal with hard to maintain html generation code. This method
            //  allows for maintaining a single cshtml partial view which is the best scenario.

            var host = Utility.GetRequiredAppSetting("ApiBaseUrl");
            var username = Utility.GetRequiredAppSetting("BasicAuthUsername");
            var password = Utility.GetRequiredAppSetting("BasicAuthPassword");

            var rc = new RestClient(host)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };

            var request = new RestRequest("webapi/data/ajax/menu");
            request.AddParameter("clientId", clientId);

            if (!string.IsNullOrEmpty(target))
                request.AddParameter("target", target);

            var response = rc.Execute(request);

            if (response.IsSuccessful)
                return new HtmlString(response.Content);
            else
                return new HtmlString(string.Format("SiteMenu Error: [{0}] {1}", (int)response.StatusCode, response.StatusDescription));
        }
    }
}
