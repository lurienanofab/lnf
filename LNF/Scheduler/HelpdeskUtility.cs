using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Email;
using LNF.Helpdesk;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Scheduler
{
    public static class HelpdeskUtility
    {
        public static IClientOrgManager ClientOrgManager => ServiceProvider.Current.Use<IClientOrgManager>();

        public static string Url
        {
            get { return ConfigurationManager.AppSettings["HelpdeskUrl"]; }
        }

        public static string ApiKey
        {
            get { return ConfigurationManager.AppSettings["HelpdeskApiKey"]; }
        }

        public static string ApiUrl
        {
            get { return Url + "/api/scheduler.php"; }
        }

        public static DataTable GetTickets(Resource resource)
        {
            if (resource == null)
                return new Helpdesk.Service(ApiUrl, ApiKey).SelectTickets();
            else
                return new Helpdesk.Service(ApiUrl, ApiKey).SelectTickets(resource.ResourceID);
        }

        public static TicketDetailResponse GetTicketDetail(int ticketId)
        {
            return new Helpdesk.Service(ApiUrl, ApiKey).SelectTicketDetail(ticketId);
        }

        public static CreateTicketResult CreateTicket(ResourceItem res, Reservation rsv, int clientId, string reservationText, string subjectText, string messageText, string ticketType)
        {
            TicketPriorty pri = TicketPriortyFromString(ticketType);

            string bodyText = GetMessageBody(res, rsv, clientId, reservationText, messageText, ticketType);

            Helpdesk.Service service = new Helpdesk.Service(ApiUrl, ApiKey);

            var client = CacheManager.Current.CurrentUser;

            var primary = ClientOrgManager.GetPrimary(client.ClientID);

            if (primary != null)
            { 
                CreateTicketResult result = service.CreateTicket
                (
                    resourceId: res.ResourceID,
                    name: ClientItem.GetDisplayName(client.LName, client.FName),
                    email: primary.Email,
                    queue: res.HelpdeskEmail,
                    subject: subjectText,
                    message: bodyText,
                    priority: pri
                );

                SendHardwareIssueEmail(res, rsv, clientId, reservationText, subjectText, messageText, pri);

                return result;
            }
            else
            {
                return new CreateTicketResult(new Exception(string.Format("Cannot find primary ClientOrg for ClientID {0}", client.ClientID)));
            }
        }

        public static void SendHardwareIssueEmail(ResourceItem res, Reservation reservation, int clientId, string reservationText, string subject, string message, TicketPriorty pri)
        {
            if (pri == TicketPriorty.HardwareIssue)
            {
                string body = GetMessageBody(res, reservation, clientId, reservationText, message, TicketPriorityToString(pri));
                SendHardwareIssueEmail(res, clientId, subject, body);
            }
        }

        public static void SendHardwareIssueEmail(ResourceItem res, int clientId, string subject, string body)
        {
            body += Environment.NewLine + Environment.NewLine + "This email has been sent by the system to notify you that a hardware issue exists on this resource and availability may be affected. Do not respond to this email. Please log into the Scheduler to view or respond to this ticket.";

            string[] recipients = GetCcEmailsForHardwareIssue(res, clientId);

            SendMessageArgs args = new SendMessageArgs()
            {
                ClientID = CacheManager.Current.CurrentUser.ClientID,
                Subject = subject,
                Body = body,
                From = SendEmail.SystemEmail,
                To = recipients
            };

            ServiceProvider.Current.Email.SendMessage(args);
        }

        public static string[] GetCcEmailsForHardwareIssue(ResourceItem resource, int clientId)
        {
            //all tool users who are not the Client, no need to send an email to themself
            ResourceClientInfo[] toolUsers = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resource.ResourceID && x.ClientID != -1 && x.ClientID != clientId).ToArray();

            //no tool engineers, they will get the email alert from the helpdesk system
            toolUsers = toolUsers.Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) == 0).ToArray();

            //GetEmail returns an array of string so we need to use SelectMany
            string[] recipients = toolUsers.Select(x => x.Email).ToArray();

            return recipients;
        }

        public static string GetMessageHeader(ResourceItem res, int clientId, string reservationText, string ticketType)
        {
            var client = CacheManager.Current.GetClient(clientId);
            string result = "Resource ID: " + res.ResourceID.ToString() + Environment.NewLine
                + "Resource Name: " + res.ResourceName + Environment.NewLine
                + "Created By: " + ClientItem.GetDisplayName(client.LName, client.FName) + Environment.NewLine
                + "Reservation: " + reservationText + Environment.NewLine
                + "Type: " + ticketType;
            return result;
        }

        public static string GetMessageBody(ResourceItem res, Reservation rsv, int clientId, string reservationText, string messageText, string ticketType)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetMessageHeader(res, clientId, reservationText, ticketType));
            if (rsv != null)
                sb.AppendLine(string.Format("Reservation History: {0}/sselonline/?view=/sselscheduler/ReservationHistory.aspx?ReservationID={1}", ServiceProvider.Current.Context.GetRequestUrl().GetLeftPart(UriPartial.Authority), rsv.ReservationID));
            sb.AppendLine(Environment.NewLine + "--------------------------------------------------" + Environment.NewLine);
            sb.AppendLine(messageText);
            return sb.ToString();
        }

        public static TicketPriorty TicketPriortyFromString(string ticketType)
        {
            TicketPriorty pri = TicketPriorty.GeneralQuestion;
            switch (ticketType)
            {
                case "General Question":
                    pri = TicketPriorty.GeneralQuestion;
                    break;
                case "Hardware Issue":
                    pri = TicketPriorty.HardwareIssue;
                    break;
                case "Process Issue":
                    pri = TicketPriorty.ProcessIssue;
                    break;
            }
            return pri;
        }

        public static string TicketPriorityToString(TicketPriorty pri)
        {
            string ticketType = "General Question";
            switch (pri)
            {
                case TicketPriorty.GeneralQuestion:
                    ticketType = "General Question";
                    break;
                case TicketPriorty.HardwareIssue:
                    ticketType = "Hardware Issue";
                    break;
                case TicketPriorty.ProcessIssue:
                    ticketType = "Process Issue";
                    break;
            }
            return ticketType;
        }

        public static TicketDetailResponse PostMessage(int ticketId, string message)
        {
            Helpdesk.Service service = new Helpdesk.Service(ApiUrl, ApiKey);
            TicketDetailResponse result = service.PostMessage(ticketId, message);
            return result;
        }

        public static string GetSchedulerHelpdeskUrl(string host, int resourceId)
        {
            string result = string.Format("http://{0}/scheduler/resource/" + resourceId.ToString(), host);
            return result;
        }
    }
}
