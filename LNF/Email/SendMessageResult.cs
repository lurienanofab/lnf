using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email
{
    public struct SendMessageResult
    {
        public static SendMessageResult SuccessResult(Exception ex, string from, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            return new SendMessageResult(true, ex, from, to, cc, bcc);
        }

        public static SendMessageResult ErrorResult(Exception ex, string from, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            return new SendMessageResult(false, ex, from, to, cc, bcc);
        }

        private SendMessageResult(bool success, Exception ex, string from, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            Success = success;
            Exception = ex;
            From = from;
            To = (to == null) ? new string[] { } : to.ToArray();
            Cc = (cc == null) ? new string[] { } : cc.ToArray();
            Bcc = (bcc == null) ? new string[] { } : bcc.ToArray();
        }

        public bool Success { get; }
        public Exception Exception { get; }
        public string From { get; }
        public string[] To { get; }
        public string[] Cc { get; }
        public string[] Bcc { get; }

        public string GetErrorMessage()
        {
            if (Exception == null)
                return string.Empty;
            else
                return Exception.Message;
        }

        /// <summary>
        /// Throws an exception if the Exception property is not null.
        /// </summary>
        public void Assert()
        {
            if (Exception != null)
                throw Exception;
        }
    }
}
