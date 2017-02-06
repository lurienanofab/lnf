using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using System;
using System.Data;
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
            this.SetErrorID(null);
            if (ex == null) return;
            string errorId = string.Empty;
            string errorMsg = this.GetErrorMessage(ex);
            int clientId = this.GetClientID();
            string clientName = this.GetDisplayName();
            string filePath = Request.Url.ToString();

            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                errorId = dba
                    .AddParameter("@Action", "Set")
                    .AddParameterIf("@ErrorMsg", !string.IsNullOrEmpty(errorMsg), errorMsg)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ClientName", !string.IsNullOrEmpty(clientName), clientName)
                    .AddParameterIf("@FilePath", !string.IsNullOrEmpty(filePath), filePath)
                    .ExecuteScalar<string>("PassError_Select");

                EventLogger.WriteToSystemLog(
                    clientId,
                    ((errorId == string.Empty) ? Guid.NewGuid() : new Guid(errorId)),
                    CommonTools.EventLogger.LogMessageTypes.Error,
                    errorMsg
                );
            }

            if (errorId != string.Empty)
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

        private void SetErrorID(string value)
        {
            try
            {
                CacheManager.Current.ErrorID = value;
            }
            catch { }
        }

        private string GetErrorID()
        {
            try
            {
                return CacheManager.Current.ErrorID;
            }
            catch
            {
                return null;
            }
        }

        private int GetClientID()
        {
            try
            {
                return CacheManager.Current.CurrentUser.ClientID;
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
                return CacheManager.Current.CurrentUser.DisplayName;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetAppName()
        {
            try
            {
                return Request.QueryString["AppName"];
            }
            catch
            {
                return string.Empty;
            }
        }

        private string DevEmails()
        {
            return string.Join(",", SendEmail.DeveloperEmails);
        }

        protected void SendError(object sender, EventArgs e)
        {
            SendButton.Enabled = false;

            string emails = this.DevEmails();
            string errorMsg = string.Empty;
            int clientId = 0;
            DateTime errorTime = DateTime.MinValue;
            string emailMsg = string.Empty;

            if (GetErrorID() != null)
            {
                Guid errorId = new Guid(GetErrorID().ToString());
                using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                {
                    DataTable dt = dba.ApplyParameters(new { Action = "Get", ErrorID = errorId }).FillDataTable("PassError_Select");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        errorMsg = dr["ErrorMsg"].ToString();
                        errorTime = Convert.ToDateTime(dr["ErrorTime"]);
                    }
                }

                string DisplayName = (string.IsNullOrEmpty(this.GetDisplayName()) ? "Unknown" : this.GetDisplayName());

                emailMsg += "Application error message from: " + DisplayName + "<br />";

                if (errorTime.Equals(DateTime.MinValue))
                    emailMsg += "Error time not available.<br />";
                else
                    emailMsg += "Error time: " + errorTime.ToString("dd-MMM-yyyy hh:mm:ss:fff") + "<br />";

                if (clientId == 0)
                    emailMsg += "<br />";
                else
                    emailMsg += " (" + this.GetClientID().ToString() + ")<br />";

                if (DescriptionTextBox.Text.Trim().Length == 0)
                    emailMsg += "No user entered description.<br /><br />";
                else
                    emailMsg += "User entered description: " + DescriptionTextBox.Text + "<br /><br />";

                if (string.IsNullOrEmpty(errorMsg))
                    emailMsg += "Error message not available.<br />";
                else
                    emailMsg += "Error message details:<br />" + errorMsg;

                string EmailSubject = "Application Error : ";

                if (!string.IsNullOrEmpty(this.GetAppName()))
                    EmailSubject += this.GetAppName();

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
