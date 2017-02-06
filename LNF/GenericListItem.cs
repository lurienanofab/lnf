using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public class GenericListItem
    {
        public object Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }

        public GenericListItem()
        {
            Value = null;
            Text = string.Empty;
            Selected = false;
        }

        public GenericListItem(object value, string text, bool selected = false)
        {
            Value = value;
            Text = text;
            Selected = selected;
        }

        public string AsHtml()
        {
            return string.Format("<option value=\"{0}\"{1}>{2}</option>\n", Value, (Selected) ? " selected" : string.Empty, Text);
        }

        public KeyValuePair<string, object> AsKeyValuePair(out bool selected)
        {
            selected = Selected;
            return new KeyValuePair<string, object>(Text, Value);
        }

        public dynamic AsDynamic(out bool selected)
        {
            selected = Selected;
            var result = new { Value = this.Value, Text = this.Text };
            return result;
        }
    }
}
