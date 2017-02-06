using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc.UI
{
    public class CheckBoxListItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public string ToolTip { get; set; }
        public bool Disabled { get; set; }

        public CheckBoxListItem()
        {
            Value = string.Empty;
            Text = string.Empty;
            Checked = false;
            ToolTip = string.Empty;
        }

        public CheckBoxListItem(string value, string text)
        {
            Value = value;
            Text = text;
            Checked = false;
            ToolTip = string.Empty;
        }

        public CheckBoxListItem(string value, string text, bool is_checked)
        {
            Value = value;
            Text = text;
            Checked = is_checked;
            ToolTip = string.Empty;
        }

        public CheckBoxListItem(string value, string text, bool is_checked, string tool_tip)
        {
            Value = value;
            Text = text;
            Checked = is_checked;
            ToolTip = tool_tip;
        }

        public string Render(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<td title=\"" + this.ToolTip + "\"><input type=\"checkbox\" id=\"" + id + "\" name=\"" + id + "\" value=\"" + this.Value + "\"" + (this.Checked ? " checked" : "") + (Disabled ? " disabled=\"disabled\"" : string.Empty) + " /><label for=\"" + id + "\">" + this.Text + "</label></td>");
            return sb.ToString();
        }
    }
}
