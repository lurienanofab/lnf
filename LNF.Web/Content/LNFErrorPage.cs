using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
using System;
using System.Web.UI.WebControls;

namespace LNF.Web.Content
{
    public abstract class LNFErrorPage : LNFPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        protected abstract Button SendButton { get; }
        protected abstract TextBox DescriptionTextBox { get; }
        protected abstract Label DoneMessageLabel { get; }
        protected abstract Literal ErrorMessageLiteral { get; }
        protected abstract HyperLink ReloadPageHyperlink { get; }

        private void LoadError()
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                LogError(ex);
                string html = @"<div class=""error-display""><div class=""error-display-title"">Exception Detail</div><div class=""error-display-content"">";
                if (ex != null)
                {
                    html += @"<div style=""font-family: arial; font-size: 10pt;""><pre>";
                    html += GetErrorMessage(ex);
                    html += @"</pre></div>";
                }
                else
                {
                    html += @"<div style=""font-style: italic; color: #999999;"">No error was found.</div>";
                }
                html += @"</div></div>";

                ErrorMessageLiteral.Text = html;
                ReloadPageHyperlink.NavigateUrl = Request.Url.ToString();
            }
        }

        private void LogError(Exception ex)
        {
            ClearErrorID();
            if (ex == null) return;
            Guid errorId = Guid.Empty;
            string errorMsg = GetErrorMessage(ex);
            int clientId = GetClientID();
            string clientName = GetDisplayName();
            string filePath = Request.Url.ToString();


            errorId = ServiceProvider.Current.Log.SetPassError(errorMsg, clientId, clientName, filePath);

            var msgId = errorId == Guid.Empty ? Guid.NewGuid() : errorId;

            EventLogger.WriteToSystemLog(clientId, msgId, LogMessageTypes.Error, errorMsg);

            if (errorId != Guid.Empty)
                SetErrorID(errorId);
        }

        private string GetErrorMessage(Exception ex)
        {
            if (ex == null) return string.Empty;
            string result = string.Empty;
            result = Request.Url.ToString();
            result += Environment.NewLine + ex.Message;
            if (ex.InnerException != null)
                result += Environment.NewLine + ex.InnerException.Message;
            if (!string.IsNullOrEmpty(ex.StackTrace))
                result += Environment.NewLine + ex.StackTrace;
            return result;
        }


        private void ClearErrorID()
        {
            ContextBase.ClearErrorID();
        }

        private void SetErrorID(Guid value)
        {
            try
            {
                ContextBase.SetErrorID(value);
            }
            catch { }
        }

        private Guid GetErrorID()
        {
            try
            {
                return ContextBase.GetErrorID();
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private int GetClientID()
        {
            try
            {
                return CurrentUser.ClientID;
            }
            catch
            {
                return 0;
            }
        }

        private string GetDisplayName()
        {
            try
            {
                return CurrentUser.DisplayName;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetAppName()
        {
            return Request.QueryString["AppName"];
        }

        private string DevEmails()
        {
            return string.Join(",", SendEmail.DeveloperEmails);
        }

        protected void SendError(object sender, EventArgs e)
        {
            SendButton.Enabled = false;

            string emails = DevEmails();
            string errorMsg = string.Empty;
            int clientId = 0;
            DateTime errorTime = DateTime.MinValue;
            string emailMsg = string.Empty;

            if (GetErrorID() != null)
            {
                Guid errorId = new Guid(GetErrorID().ToString());

                IPassError err = ServiceProvider.Current.Log.GetPassError(errorId);

                if (err != null)
                {
                    errorMsg = err.ErrorMsg;
                    errorTime = err.ErrorTime;
                }

                string temp = GetDisplayName();
                string displayName = (string.IsNullOrEmpty(temp) ? "Unknown" : temp);

                emailMsg += $"Application error message from: {displayName}<br />";

                if (errorTime.Equals(DateTime.MinValue))
                    emailMsg += "Error time not available.<br />";
                else
                    emailMsg += $"Error time: {errorTime:dd-MMM-yyyy hh:mm:ss:fff}<br />";

                if (clientId == 0)
                    emailMsg += "<br />";
                else
                    emailMsg += $" ({GetClientID()})<br />";

                if (DescriptionTextBox.Text.Trim().Length == 0)
                    emailMsg += "No user entered description.<br /><br />";
                else
                    emailMsg += $"User entered description: {DescriptionTextBox.Text}<br /><br />";

                if (string.IsNullOrEmpty(errorMsg))
                    emailMsg += "Error message not available.<br />";
                else
                    emailMsg += $"Error message details:<br />{errorMsg}";

                string EmailSubject = "Application Error : ";

                if (!string.IsNullOrEmpty(GetAppName()))
                    EmailSubject += GetAppName();

                if (!string.IsNullOrEmpty(emails))
                {
                    SendEmail.Email("lnf-it@umich.edu", emails, false, EmailSubject, emailMsg);
                    DoneMessageLabel.Text = "Your message has been sent to the application developers.";
                }
                else
                {
                    DoneMessageLabel.Text = "A problem occurred: Developer emails could not be found.";
                }
            }
            else
            {
                DoneMessageLabel.Text = "A problem occurred: No error was found.";
            }
        }
    }
}
