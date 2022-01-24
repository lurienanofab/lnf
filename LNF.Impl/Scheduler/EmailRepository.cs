using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LNF.Impl.Scheduler
{
    public class EmailRepository : RepositoryBase, IEmailRepository
    {
        protected IClientRepository Client { get; }

        public EmailRepository(ISessionManager mgr, IClientRepository client) : base(mgr)
        {
            Client = client;
        }

        //Email users who want to be notified of open reservation slots
        public EmailOnOpenReservationsProcessResult EmailOnOpenReservations(int reservationId, DateTime startDate, DateTime endDate)
        {
            var startedAt = DateTime.Now;
            var data = new List<string>();
            var totalEmailsSent = 0;

            var rsv = Require<ReservationInfo>(reservationId);

            IList<ResourceClientInfo> emailClients = Session.SelectEmailClients(rsv.ResourceID).ToList();

            foreach (ResourceClientInfo rc in emailClients)
            {
                //If the open slot falls into the client's default working hours and default working days
                DateTime today = DateTime.Now;
                int clientId = rc.ClientID;
                ClientSetting cs = Session.Get<ClientSetting>(clientId);
                string[] workDays = cs.WorkDays.Split(',');
                if (startDate < today.AddHours(Convert.ToDouble(cs.EndHour)) && endDate > today.AddHours(Convert.ToDouble(cs.BeginHour)) && workDays[(int)today.DayOfWeek] == "1")
                {
                    var client = Session.Get<Client>(clientId);
                    IEnumerable<string> recipient = Client.ActiveEmails(client.ClientID);
                    string subject = "Open reservation slot for " + rsv.ResourceName;
                    string body = rsv.ResourceName + " just became available for reservation from "
                        + startDate + " to " + endDate + ".<br /><br />"
                        + "If you wish to reserve this resource, please sign up quickly.<br /><br />";

                    try
                    {
                        SendEmail.SendSystemEmail("LNF.Scheduler.EmailUtility.EmailOnOpenReservations", subject, body, recipient);
                        totalEmailsSent += 1;
                        data.Add($"Open reservation slot: Email sent to {recipient}, Resource: {rsv.ResourceName}, BeginDateTime: {startDate}, EndDateTime: {endDate}");
                    }
                    catch (Exception ex)
                    {
                        data.Add($"Open reservation slot: ERROR sending email to {recipient}, Resource: {rsv.ResourceName}, BeginDateTime: {startDate}, EndDateTime: {endDate}, Message: {ex.Message}");
                    }
                }
            }

            var result = new EmailOnOpenReservationsProcessResult(startedAt)
            {
                TotalEmailsSent = totalEmailsSent
            };

            return result;
        }

        public void EmailOnSaveHistory(int reservationId, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser, int clientId)
        {
            if (updateCharges || updateAccount || updateNotes)
            {
                var rsv = Require<ReservationInfo>(reservationId);

                string subject;
                List<string> toAddr = new List<string>();
                string emailForgivenCharge = ConfigurationManager.AppSettings["EmailForgivenCharge"];
                StringBuilder body = new StringBuilder();
                TimeSpan ts = rsv.ChargeEndDateTime - rsv.ChargeBeginDateTime;
                string fromAddr = Properties.Current.SchedulerEmail;
                if (sendToUser) toAddr.AddRange(Client.ActiveEmails(rsv.ClientID));
                if (!string.IsNullOrEmpty(emailForgivenCharge))
                    toAddr.Add(emailForgivenCharge);
                if (toAddr.Count == 0) return;
                subject = $"{SendEmail.CompanyName} - Reservation Modified";
                body.AppendLine("This is an automatically generated email to let you know that:");
                body.AppendLine("<ol>");
                if (updateCharges)
                    body.AppendLine(string.Format("<li>{0} of the charges have been forgiven by one of the tool engineers.</li>", (1 - rsv.ChargeMultiplier).ToString("0.0%")));
                if (updateAccount)
                    body.AppendLine(string.Format("<li>The account has been changed to {0}.</li>", rsv.AccountName));
                if (updateNotes)
                    body.AppendLine(string.Format("<li>The following notes have been entered:<div style=\"margin-left: 10px; padding-left: 5px; border-left: 2px solid #CCCCCC;; width: 400px; font-family: 'courier new';\">{0}</div></li>", rsv.Notes));
                body.AppendLine("</ol>");
                body.AppendLine("Reservation Detail:");
                body.AppendLine("<div style=\"margin-left: 10px;\">");
                body.AppendLine(string.Format("Run number: {0}<br />", rsv.ReservationID));
                body.AppendLine(string.Format("Tool: {0}<br />", rsv.ResourceName));
                body.AppendLine(string.Format("Time: from {0} to {1}<br />", rsv.ChargeBeginDateTime, rsv.ChargeEndDateTime));
                body.AppendLine(string.Format("Total minutes: {0}<br />", ts.TotalMinutes.ToString("0.##")));
                body.AppendLine("</div>");

                if (toAddr.Count == 0) return;

                SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnSaveHistory", subject, body.ToString(), fromAddr, toAddr);
            }
        }

        //Send email to reserver when his/her reservation is created
        public void EmailOnUserCreate(int reservationId, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            ClientSetting reserverSetting = Session.Get<ClientSetting>(rsv.ClientID);
            if (reserverSetting.EmailCreateReserv.Value || rsv.ClientID != clientId)
            {
                string fromAddr, subject, body;
                IEnumerable<string> toAddr;
                fromAddr = Properties.Current.SchedulerEmail;
                toAddr = Client.ActiveEmails(rsv.ClientID);
                subject = $"{SendEmail.CompanyName} - Reservation Created";
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been created.",
                    Clients.GetDisplayName(rsv.LName, rsv.FName),
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.ResourceName,
                    Environment.NewLine);

                if (toAddr.Count() == 0) return;

                SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnUserCreate", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when his/her reservation is updated, either by reserver or by TE
        public void EmailOnUserUpdate(int reservationId, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            ClientSetting reserverSetting = Session.Get<ClientSetting>(rsv.ClientID);
            if (reserverSetting.EmailModifyReserv.Value || rsv.ClientID != clientId)
            {
                string fromAddr, subject, body;
                fromAddr = Properties.Current.SchedulerEmail;
                IEnumerable<string> toAddr = Client.ActiveEmails(rsv.ClientID);
                subject = $"{SendEmail.CompanyName} - Reservation Updated";
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been updated.",
                    Clients.GetDisplayName(rsv.LName, rsv.FName),
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.ResourceName,
                    Environment.NewLine);

                if (toAddr.Count() == 0) return;

                SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnUserUpdate", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when his/her reservation is deleted, either by reserver or by TE
        public void EmailOnUserDelete(int reservationId, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            ClientSetting reserverSetting = Session.Get<ClientSetting>(rsv.ClientID);
            if (reserverSetting.EmailDeleteReserv.Value || rsv.ClientID != clientId)
            {
                string fromAddr, subject, body;
                fromAddr = Properties.Current.SchedulerEmail;
                IEnumerable<string> toAddr = Client.ActiveEmails(rsv.ClientID);
                subject = $"{SendEmail.CompanyName} - Reservation Deleted";
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been deleted.",
                    Clients.GetDisplayName(rsv.LName, rsv.FName),
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.ResourceName,
                    Environment.NewLine);

                if (toAddr.Count() == 0) return;

                SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnUserDelete", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when reservation is overrode by TE
        public void EmailOnToolEngDelete(int reservationId, IClient toolEng, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            string displayName = Clients.GetDisplayName(rsv.LName, rsv.FName);
            string fromAddr, subject, body;
            fromAddr = toolEng.Email;
            IEnumerable<string> toAddr = Client.ActiveEmails(rsv.ClientID);
            subject = $"{SendEmail.CompanyName} - Reservation for {displayName} has been removed";
            body = string.Format("{0}{7}{7}Your reservation beginning at {1} and ending at {2} for resource {3} has been removed because {4}, who has administrator or tool engineer access, has made a reservation that overrides yours.{7}{7}If you have any questions please contact {4} at {5}.{7}{7}Notes from reserver:{7}{6}",
                displayName,
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.ResourceName,
                toolEng.DisplayName,
                fromAddr,
                rsv.Notes,
                Environment.NewLine);

            if (toAddr.Count() == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnToolEngDelete", subject, body, fromAddr, toAddr);
        }

        //Send email to invitees when they are invited to a reservation
        public void EmailOnInvited(int reservationId, IEnumerable<Invitee> invitees, int clientId, ReservationModificationType modificationType = ReservationModificationType.Created)
        {
            if (invitees == null) return;

            var invited = invitees.Where(x => !x.Removed).ToArray();

            if (invited.Length == 0) return;

            var rsv = Require<ReservationInfo>(reservationId);
            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = $"{SendEmail.CompanyName} - Reservation Invitation";

            //reservationType
            string invitedModifiedText;

            if (modificationType == ReservationModificationType.Created)
                invitedModifiedText = "invited you to a reservation";
            else
                invitedModifiedText = "modified a reservation to which you are invited";

            body = string.Format("{0} has {1} for resource {2}.", Clients.GetDisplayName(rsv.LName, rsv.FName), invitedModifiedText, rsv.ResourceName);
            body += Environment.NewLine + string.Format("- Begin time: {0}", rsv.BeginDateTime.ToString(Reservation.DateFormat));
            body += Environment.NewLine + string.Format("- End time: {0}", rsv.EndDateTime.ToString(Reservation.DateFormat));

            foreach (var ri in invited)
            {
                //Send email if invitee wants to receive email
                ClientSetting inviteeSetting = Session.Get<ClientSetting>(ri.InviteeID);
                if (inviteeSetting != null)
                {
                    if (inviteeSetting.EmailInvited.Value)
                    {
                        var primary = Client.GetPrimary(ri.InviteeID);
                        if (primary != null)
                            toAddr.Add(primary.Email);
                    }
                }
            }

            if (toAddr.Count == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnInvited", subject, body, fromAddr, toAddr);
        }

        //Send email to invitees when they are uninvited to a reservation
        public void EmailOnUninvited(int reservationId, IEnumerable<Invitee> invitees, int clientId)
        {
            if (invitees == null) return;

            var removed = invitees.Where(x => x.Removed).ToArray();

            if (removed.Length == 0) return;

            var rsv = Require<ReservationInfo>(reservationId);

            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();

            fromAddr = Properties.Current.SchedulerEmail;
            subject = $"{SendEmail.CompanyName} - Reservation Invitation Canceled";
            body = string.Format("Your invitation to a reservation made by {0} on {1} for resource {2} has been canceled.",
                Clients.GetDisplayName(rsv.LName, rsv.FName),
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.ResourceName);

            foreach (var ri in removed)
            {
                //Send email if invitee wants to receive email
                ClientSetting inviteeSetting = Session.Get<ClientSetting>(ri.InviteeID);
                if (inviteeSetting != null && inviteeSetting.IsValid())
                {
                    if (inviteeSetting.EmailInvited.Value)
                    {
                        var primary = Client.GetPrimary(ri.InviteeID);
                        if (primary != null)
                            toAddr.Add(primary.Email);
                    }
                }
            }

            if (toAddr.Count == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnUninvited", subject, body, fromAddr, toAddr);
        }

        //Email users who want to be notified of open reservation slots
        public void EmailOnOpenSlot(int reservationId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int clientId)
        {
            IList<ResourceClientInfo> clients = null;
            string footer = string.Empty;

            var rsv = Require<ReservationInfo>(reservationId);

            if (notifyType == EmailNotify.Always)
            {
                clients = Session.SelectNotifyOnCancelClients(rsv.ResourceID).ToList();
                footer = string.Format("Sent to NotifyOnCancel clients, ReservationID: {0}", rsv.ReservationID);
            }
            else if (notifyType == EmailNotify.OnOpening)
            {
                clients = Session.SelectNotifyOnOpeningClients(rsv.ResourceID).ToList();
                footer = string.Format("Sent to NotifyOnOpening clients, ReservationID: {0}", rsv.ReservationID);
            }

            if (clients == null || clients.Count == 0) return;

            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = $"{SendEmail.CompanyName} - Open reservation slot for {rsv.ResourceName}";
            body = string.Format("{0} just became available for reservation from {1} to {2}.{4}{4}If you wish to reserve this resource, please sign up quickly.{4}{4}--------------------{4}{4}{3}",
                rsv.ResourceName,
                beginDateTime.ToString(Reservation.DateFormat),
                endDateTime.ToString(Reservation.DateFormat),
                footer,
                Environment.NewLine
            );

            foreach (ResourceClientInfo rc in clients)
            {
                if (!rc.IsEveryone())
                    toAddr.Add(rc.Email);
            }

            if (toAddr.Count == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnOpenSlot", subject, body, fromAddr, toAddr);
        }

        public void EmailOnPracticeRes(int reservationId, string inviteeName, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);

            IList<ResourceClientInfo> clients = Session.SelectNotifyOnPracticeRes(rsv.ResourceID).ToList();
            if (clients == null || clients.Count == 0) return;

            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = $"{SendEmail.CompanyName} - {Clients.GetDisplayName(rsv.LName, rsv.FName)} has made Practice reservation on {rsv.ResourceName}";
            body = string.Format("Practice reservation from {0} to {1}. The invitee is {2}.",
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                inviteeName);

            foreach (ResourceClientInfo rc in clients)
            {
                if (!rc.IsEveryone())
                    toAddr.Add(rc.Email);
            }

            if (toAddr.Count == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnPracticeRes", subject, body, fromAddr, toAddr);
        }

        //Email reservers when their reservations are canceled because TE changed granularity.
        public void EmailOnCanceledByResource(int reservationId, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            string fromAddr, subject, body;
            fromAddr = Properties.Current.SchedulerEmail;
            IEnumerable<string> toAddr = Client.ActiveEmails(rsv.ClientID);

            subject = $"{SendEmail.CompanyName} - Reservation Canceled";
            body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been canceled due to an update in the resource configuration.{4}{4}If you have any questions please contact the tool engineer.",
                Clients.GetDisplayName(rsv.LName, rsv.FName),
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.ResourceName,
                Environment.NewLine);

            if (toAddr.Count() == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnCanceledByResource", subject, body, fromAddr, toAddr);
        }

        //Email reservers when their reservations are canceled because TE needs to repair resource.
        public void EmailOnCanceledByRepair(int reservationId, bool isRemoved, string state, string notes, DateTime repairEndDateTime, int clientId)
        {
            var rsv = Require<ReservationInfo>(reservationId);
            string fromAddr, subject, body;
            fromAddr = Properties.Current.SchedulerEmail;
            IEnumerable<string> toAddr = Client.ActiveEmails(rsv.ClientID);

            subject = $"{SendEmail.CompanyName} - Reservation Canceled";
            body = string.Format("{0}{8}{8}Your reservation beginning at {1} and ending at {2} for resource {3} has been {4} because this resource has been marked '{5}' until {6}.{8}{8}The reason for the change:{8}{7}{8}{8}If you have any questions, please contact the tool engineer.",
                Clients.GetDisplayName(rsv.LName, rsv.FName),
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.ResourceName,
                (isRemoved) ? "removed" : "forced to end",
                state,
                repairEndDateTime.ToString(Reservation.DateFormat),
                notes,
                Environment.NewLine);

            if (toAddr.Count() == 0) return;

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnCanceledByRepair", subject, body, fromAddr, toAddr);
        }

        /// <summary>
        /// Email reservers when tool engineers have forgiven charges to their reservations.
        /// </summary>
        public void EmailOnForgiveCharge(int reservationId, double forgiveAmount, bool sendToUser, int clientId)
        {
            // clientId is for the user sending the email (i.e. staff doing the forgiving)

            var rsv = Require<ReservationInfo>(reservationId);
            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            string emailForgivenCharge = ConfigurationManager.AppSettings["EmailForgivenCharge"];
            TimeSpan ts = rsv.ChargeEndDateTime - rsv.ChargeBeginDateTime;
            fromAddr = Properties.Current.SchedulerEmail;

            if (sendToUser) toAddr.AddRange(Client.ActiveEmails(rsv.ClientID));

            if (!string.IsNullOrEmpty(emailForgivenCharge))
                toAddr.AddRange(emailForgivenCharge.Split(','));

            if (toAddr.Count == 0) return;

            var note = ConfigurationManager.AppSettings["ForgiveChargeNote"];

            subject = $"{SendEmail.CompanyName} - Reservation Charges Forgiven";
            body = $"This is an automatically generated email to let you know that {forgiveAmount}% of the charges on run number {rsv.ReservationID} have been forgiven by one of the tool engineers. {note}<br /><br />Reservation Detail:<ul><li>Tool: {rsv.ResourceName}</li><li>Chargeable Time: from {rsv.ChargeBeginDateTime.ToString(Reservation.DateFormat)} to {rsv.ChargeEndDateTime.ToString(Reservation.DateFormat)}</li><li>Total Chargeable Minutes: {ts.TotalMinutes:0.##}</li></ul>";

            SendEmail.Send(clientId, "LNF.Scheduler.EmailUtility.EmailOnForgiveCharge", subject, body, fromAddr, toAddr);
        }
    }
}
