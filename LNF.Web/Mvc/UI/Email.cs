using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LNF.Web.Mvc.UI
{
    public class Email
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public object HtmlAttributes { get; set; }

        public IHtmlString Render()
        {
            if (string.IsNullOrEmpty(Name))
                throw new InvalidOperationException("Name is required.");

            TagBuilder builder = new TagBuilder("input");
            builder.Attributes.Add("id", Name);
            builder.Attributes.Add("name", Name);
            builder.Attributes.Add("type", "email");
            builder.Attributes.Add("value", GetValue());

            if (HtmlAttributes != null)
            {
                RouteValueDictionary attribs = HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes);
                foreach (var kvp in attribs)
                    builder.Attributes.Add(kvp.Key, kvp.Value.ToString());
            }

            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        private string GetValue()
        {
            return (Value == null) ? string.Empty : Value.ToString();
        }
    }
}
