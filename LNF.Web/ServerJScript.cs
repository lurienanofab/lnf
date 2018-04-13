using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web
{
    public static class ServerJScript
    {
        public static string JSEncode(string message)
        {
            //*** Please Note xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx****
            //      The compiler interprets:
            //          \n is Line Feed (LF)
            //          \r is Carriage Return (CR)
            //          \t is Horizontal Tab (HT)
            //          \ is a special escape charater, not the backslash character which must be expressed as \\ inside a string literal (i.e. "\\")
            //          \\ is backslash (BACKSLASH)
            //
            //      \n is a line feed, NOT a slash character followed by the n character, so
            //      the code "hello\nworld".Replace("\\", "\\\\") does nothing because there is
            //      no backslash character to replace (\n is LF, "\\" is BACKSLASH).
            //
            //      Good:   "alert('hello\nworld')".Replace("\n", "\\n")     equals "alert('hello\\nworld')"   and renders alert('hello\nworld')    <== javascript displays a dialog with two lines of text
            //      Bad:    "alert('hello\nworld')".Replace("\\", "\\\\")    equals "alert('hello\nworld')"    and renders alert('hello             <== javascript throws an error
            //                                                                                                             world')
            //
            //      So the character that comes after hello in "hello\nworld" is not \ - it is \n (LF).
            //      Therefore you can not change \n to \\n by replacing "\\" with "\\\\". However this would
            //      change "hello\\world" into "hello\\\\world" (when printed: hello\world into hello\\world).
            //      Also, in C# Environment.NewLine is \r\n (aka CRLF).
            //
            //      Also see LNF.Web.Tests.ServerJScriptTests.CanJSEncode for unit tests
            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            //1) convert all \r\n to just \n, (CRLF to LF)
            message = message.Replace("\r", string.Empty);
            //2) convert all \n to \\n (LF to \n)
            message = message.Replace("\n", "\\n");
            //3) convert all \t to \\t (HT to \t)
            message = message.Replace("\t", "\\t");
            //4) convert all ' to \'
            message = message.Replace("'", "\\'");

            return message;
        }

        public static void JSAlert(Page aspxPage, string Message)
        {
            string script = string.Format("<script type=\"text/javascript\">alert('{0}');</script>", JSEncode(Message));
            if (!aspxPage.ClientScript.IsStartupScriptRegistered("jsAlert"))
                aspxPage.ClientScript.RegisterStartupScript(aspxPage.GetType(), "jsAlert", script);
        }

        public static void AddToolTip(WebControl control, string tooltip, string caption = "", bool cancelBubble = false)
        {
            string mouseover = string.Empty;

            if (cancelBubble)
                mouseover = "event.cancelBubble=true;";

            mouseover += "return overlib('" + JSEncode(tooltip) + "'";

            if (!string.IsNullOrEmpty(caption))
                mouseover += ", CAPTION, '" + JSEncode(caption) + "'";

            mouseover += ");";

            control.Attributes.Add("onmouseover", mouseover);
            control.Attributes.Add("onmouseout", "return nd();");
        }
    }
}
