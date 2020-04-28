using LNF.CommonTools;
using LNF.Data;
using LNF.Helpdesk;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Scheduler
{
    public static class HelpdeskUtility
    {
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

        public static DataTable GetTickets(IResource resource)
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

        public static CreateTicketResult CreateTicket(IClient currentUser, IResource res, IReservation rsv, int clientId, string reservationText, string subjectText, string messageText, string ticketType, Uri requestUri)
        {
            TicketPriorty pri = TicketPriortyFromString(ticketType);

            string bodyText = GetMessageBody(res, rsv, clientId, reservationText, messageText, ticketType, requestUri);

            Helpdesk.Service service = new Helpdesk.Service(ApiUrl, ApiKey);

            IClient primary;

            if (currentUser.PrimaryOrg)
                primary = currentUser;
            else
                primary = ServiceProvider.Current.Data.Client.GetPrimary(currentUser.ClientID);

            if (primary != null)
            {
                CreateTicketResult result = service.CreateTicket
                (
                    resourceId: res.ResourceID,
                    name: currentUser.DisplayName,
                    email: primary.Email,
                    queue: res.HelpdeskEmail,
                    subject: subjectText,
                    message: bodyText,
                    priority: pri
                );

                SendHardwareIssueEmail(res, rsv, clientId, reservationText, subjectText, messageText, pri, requestUri);

                return result;
            }
            else
            {
                return new CreateTicketResult(new Exception($"Cannot find primary ClientOrg for ClientID {currentUser.ClientID}"));
            }
        }

        public static void SendHardwareIssueEmail(IResource res, IReservation reservation, int clientId, string reservationText, string subject, string message, TicketPriorty pri, Uri requestUri)
        {
            if (pri == TicketPriorty.HardwareIssue)
            {
                string body = GetMessageBody(res, reservation, clientId, reservationText, message, TicketPriorityToString(pri), requestUri);
                SendHardwareIssueEmail(res, clientId, subject, body);
            }
        }

        public static void SendHardwareIssueEmail(IResource res, int clientId, string subject, string body)
        {
            body += Environment.NewLine + Environment.NewLine + "This email has been sent by the system to notify you that a hardware issue exists on this resource and availability may be affected. Do not respond to this email. Please log into the Scheduler to view or respond to this ticket.";
            string[] recipients = GetCcEmailsForHardwareIssue(res, clientId);
            SendEmail.SendSystemEmail("LNF.Scheduler.HelpdeskUtility.SendHardwareIssueEmail", subject, body, recipients);
        }

        public static string[] GetCcEmailsForHardwareIssue(IResource resource, int clientId)
        {
            //all tool users who are not the Client, no need to send an email to themself
            IResourceClient[] toolUsers = ServiceProvider.Current.Scheduler.Resource.GetResourceClients(resource.ResourceID).Where(x => !x.IsClientOrEveryone(clientId)).ToArray();

            //no tool engineers, they will get the email alert from the helpdesk system
            toolUsers = toolUsers.Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) == 0).ToArray();

            //GetEmail returns an array of string so we need to use SelectMany
            string[] recipients = toolUsers.Select(x => x.Email).ToArray();

            return recipients;
        }

        public static string GetMessageHeader(IResource res, int clientId, string reservationText, string ticketType)
        {
            var client = ServiceProvider.Current.Data.Client.GetClient(clientId);
            string result = "Resource ID: " + res.ResourceID.ToString() + Environment.NewLine
                + "Resource Name: " + res.ResourceName + Environment.NewLine
                + "Created By: " + Clients.GetDisplayName(client.LName, client.FName) + Environment.NewLine
                + "Reservation: " + reservationText + Environment.NewLine
                + "Type: " + ticketType;
            return result;
        }

        public static string GetMessageBody(IResource res, IReservation rsv, int clientId, string reservationText, string messageText, string ticketType, Uri requestUri)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetMessageHeader(res, clientId, reservationText, ticketType));
            if (rsv != null)
                sb.AppendLine(string.Format("Reservation History: {0}/sselonline/?view=/sselscheduler/ReservationHistory.aspx?ReservationID={1}", requestUri.GetLeftPart(UriPartial.Authority), rsv.ReservationID));
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
