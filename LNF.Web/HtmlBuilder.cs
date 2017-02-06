using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LNF.Web
{
    public class HtmlBuilder : IDisposable
    {
        private static readonly Object locker = new Object();

        private string _tag;
        private string _innerHtml;
        private StringBuilder _sb;
        private HtmlTextWriter _writer;
        private bool _selfClosing;

        private HtmlBuilder(StringBuilder sb, string tag, object htmlAttributes, string innerHtml = "")
        {
            _sb = sb;
            _writer = null;
            _tag = tag;
            _innerHtml = innerHtml;

            if (innerHtml == null)
                _selfClosing = true;
            else
                _selfClosing = false;

            string html = string.Format("<{0}", tag);

            if (htmlAttributes != null)
            {
                if (!HtmlBuilder.IsAnonymousType(htmlAttributes.GetType()))
                    throw new Exception("HtmlBuilder expects htmlAttributes to be an anonymous object");

                var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

                foreach (var kvp in dict)
                    html += string.Format(" {0}=\"{1}\"", kvp.Key, kvp.Value);
            }

            html += ">";

            _sb.Append(html);

            if (!string.IsNullOrEmpty((_innerHtml == null) ? "" : _innerHtml.ToString()))
                _sb.Append(_innerHtml);
        }

        private HtmlBuilder(HtmlTextWriter writer, string tag, object htmlAttributes, string innerHtml)
        {
            _sb = null;
            _writer = writer;
            _tag = tag;
            _innerHtml = innerHtml;

            if (innerHtml == null)
                _selfClosing = true;
            else
                _selfClosing = false;

            _writer.WriteBeginTag(tag);

            if (htmlAttributes != null)
            {
                if (!HtmlBuilder.IsAnonymousType(htmlAttributes.GetType()))
                    throw new Exception("HtmlBuilder expects htmlAttributes to be an anonymous object");

                var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

                foreach (var kvp in dict)
                    _writer.WriteAttribute(kvp.Key, kvp.Value.ToString());
            }

            _writer.Write(HtmlTextWriter.TagRightChar);

            if (!string.IsNullOrEmpty((_innerHtml == null) ? "" : _innerHtml.ToString()))
                _writer.Write(_innerHtml);
        }

        public static void Append(StringBuilder sb1, StringBuilder sb2)
        {
            HtmlBuilder.Append(sb1, sb2.ToString());
        }

        public static void Append(StringBuilder sb, string text)
        {
            sb.Append(text);
        }

        public static void Append(HtmlTextWriter writer, string text)
        {
            writer.Write(text);
        }

        public static HtmlBuilder Start(StringBuilder sb, string tag, object htmlAttributes = null)
        {
            HtmlBuilder bldr = new HtmlBuilder(sb, tag, htmlAttributes, "");
            bldr._sb.Append(Environment.NewLine);
            return bldr;
        }

        public static HtmlBuilder Start(HtmlTextWriter writer, string tag, object htmlAttributes = null)
        {
            HtmlBuilder bldr = new HtmlBuilder(writer, tag, htmlAttributes, "");
            bldr._writer.Write(writer.NewLine);
            return bldr;
        }

        public static void Create(StringBuilder sb, params string[] tags)
        {
            foreach (string t in tags)
                Create(sb, t);
        }

        public static void Create(StringBuilder sb, string tag, object htmlAttributes = null, string innerHtml = "")
        {
            var bldr = new HtmlBuilder(sb, tag, htmlAttributes, innerHtml);
            bldr.Dispose();
        }

        public static void Create(HtmlTextWriter writer, params string[] tags)
        {
            foreach (string t in tags)
                Create(writer, t);
        }

        public static void Create(HtmlTextWriter writer, string tag, object htmlAttributes = null, string innerHtml = "")
        {
            using (new HtmlBuilder(writer, tag, htmlAttributes, innerHtml)) { }
        }

        public void Dispose()
        {
            lock (locker)
            {
                if (_sb != null)
                {
                    string content = _sb.ToString();
                    _sb.Clear();

                    if (_selfClosing)
                    {
                        if (content.EndsWith(">") || content.EndsWith(">" + Environment.NewLine))
                            content = content.TrimEnd(new char[] { '>', '\r', '\n' });
                        content += " />";
                    }
                    else
                        content += string.Format("</{0}>", _tag);

                    _sb.AppendLine(content);
                }

                if (_writer != null)
                {
                    if (_selfClosing)
                        _writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    else
                        _writer.WriteEndTag(_tag);
                }
            }
        }

        //http://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
        public static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}