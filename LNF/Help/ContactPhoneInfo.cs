using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LNF.Help
{
    public class ContactPhoneInfo
    {
        public ContactPhoneInfo(string phone)
        {
            _Text = string.Empty;
            _AreaCode = string.Empty;
            _Prefix = string.Empty;
            _LineNumber = string.Empty;
            _ParseErrorMessage = string.Empty;

            if (!string.IsNullOrEmpty(phone) && phone != "--")
            {
                MatchCollection matches = Regex.Matches(phone, "^([0-9]{3})-([0-9]{3})-([0-9]{4})$");
                if (matches.Count == 1)
                {
                    Match m = matches[0];
                    if (m.Groups.Count == 4)
                    {
                        _Text = m.Groups[0].Value;
                        _AreaCode = m.Groups[1].Value;
                        _Prefix = m.Groups[2].Value;
                        _LineNumber = m.Groups[3].Value;
                    }
                    else
                    {
                        _ParseErrorMessage = "Invalid phone number.";
                    }
                }
                else
                {
                    _ParseErrorMessage = "Invalid phone number.";
                }
            }
        }

        private string _Text;
        private string _AreaCode;
        private string _Prefix;
        private string _LineNumber;
        private string _ParseErrorMessage;

        public string Text { get { return _Text; } }
        public string AreaCode { get { return _AreaCode; } }
        public string Prefix { get { return _Prefix; } }
        public string LineNumber { get { return _LineNumber; } }
        public string ParseErrorMessage { get { return _ParseErrorMessage; } }

        public bool Error
        {
            get { return !string.IsNullOrEmpty(_ParseErrorMessage); }
        }
    }
}
