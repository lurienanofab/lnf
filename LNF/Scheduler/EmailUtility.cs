﻿using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LNF.Scheduler
{
    public static class EmailUtility
    {
        //Email users who want to be notified of open reservation slots
        public static int EmailOnOpenReservations(int resourceId, DateTime startDate, DateTime endDate)
        {
            int count = 0;

            ResourceModel res = CacheManager.Current.GetResource(resourceId);

            using (var timer = LogTaskTimer.Start("EmailUtility.EmailOnOpenReservations", "resourceId = {0}, startDate = '{1}', endDate = '{2}'", () => new object[] { res.ResourceID, startDate, endDate }))
            {
                IList<ResourceClient> emailClients = ResourceClientUtility.SelectEmailClients(res.ResourceID).ToList();

                foreach (ResourceClient rc in emailClients)
                {
                    //If the open slot falls into the client's default working hours and default working days
                    DateTime today = DateTime.Now;
                    int clientId = rc.ClientID;
                    ClientSetting cs = DA.Current.Single<ClientSetting>(clientId);
                    string[] workDays = cs.WorkDays.Split(',');
                    if (startDate < today.AddHours(Convert.ToDouble(cs.EndHour)) && endDate > today.AddHours(Convert.ToDouble(cs.BeginHour)) && workDays[(int)today.DayOfWeek] == "1")
                    {
                        IEnumerable<string> recipient = DA.Current.Single<Client>(clientId).ActiveEmails();
                        string subject = "Open reservation slot for " + res.ResourceName;
                        string body = res.ResourceName + " just became available for reservation from "
                            + startDate + " to " + endDate + ".<br /><br />"
                            + "If you wish to reserve this resource, please sign up quickly.<br /><br />";
                        Providers.Email.SendMessage(0, "LNF.Scheduler.EmailUtility.EmailOnOpenReservations(Resource resource, DateTime startDate, DateTime endDate)", subject, body, SendEmail.SystemEmail, recipient, isHtml: true);
                        timer.AddData("Open reservation slot: Email sent to {0}, Resource: {1}, BeginDateTime: {2}, EndDateTime: {3}", recipient, res.ResourceName, startDate, endDate);
                    }
                }

                return count;
            }
        }

        public static void EmailOnSaveHistory(Reservation rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser)
        {
            if (updateCharges || updateAccount || updateNotes)
            {
                string subject;
                List<string> toAddr = new List<string>();
                string emailForgivenCharge = ConfigurationManager.AppSettings["EmailForgivenCharge"];
                StringBuilder body = new StringBuilder();
                TimeSpan ts = ReservationUtility.GetEndDateTime(rsv) - ReservationUtility.GetBeginDateTime(rsv);
                string fromAddr = Properties.Current.SchedulerEmail;
                if (sendToUser) toAddr.AddRange(rsv.Client.ActiveEmails());
                if (!string.IsNullOrEmpty(emailForgivenCharge))
                    toAddr.Add(emailForgivenCharge);
                if (toAddr.Count == 0) return;
                subject = string.Format("{0} - Reservation Modified", Providers.Email.CompanyName);
                body.AppendLine("This is an automatically generated email to let you know that:");
                body.AppendLine("<ol>");
                if (updateCharges)
                    body.AppendLine(string.Format("<li>{0} of the charges have been forgiven by one of the tool engineers.</li>", (1 - rsv.ChargeMultiplier).ToString("0.0%")));
                if (updateAccount)
                    body.AppendLine(string.Format("<li>The account has been changed to {0}.</li>", rsv.Account.Name));
                if (updateNotes)
                    body.AppendLine(string.Format("<li>The following notes have been entered:<div style=\"margin-left: 10px; padding-left: 5px; border-left: 2px solid #CCCCCC;; width: 400px; font-family: 'courier new';\">{0}</div></li>", rsv.Notes));
                body.AppendLine("</ol>");
                body.AppendLine("Reservation Detail:");
                body.AppendLine("<div style=\"margin-left: 10px;\">");
                body.AppendLine(string.Format("Run number: {0}<br />", rsv.ReservationID));
                body.AppendLine(string.Format("Tool: {0}<br />", rsv.Resource.ResourceName));
                body.AppendLine(string.Format("Time: from {0} to {1}<br />", ReservationUtility.GetBeginDateTime(rsv), ReservationUtility.GetEndDateTime(rsv)));
                body.AppendLine(string.Format("Total minutes: {0}<br />", ts.TotalMinutes.ToString("0.##")));
                body.AppendLine("</div>");

                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnSaveHistory(Reservation rsv, bool updateCharges, bool updateAccount, bool updateNotes, bool sendToUser)", subject, body.ToString(), fromAddr, toAddr, isHtml: true);
            }
        }

        //Send email to reserver when his/her reservation is created
        public static void EmailOnUserCreate(Reservation rsv)
        {
            ClientSetting reserverSetting = DA.Current.Single<ClientSetting>(rsv.Client.ClientID);
            if (reserverSetting.EmailCreateReserv.Value || rsv.Client.ClientID != CacheManager.Current.CurrentUser.ClientID)
            {
                string fromAddr, subject, body;
                IEnumerable<string> toAddr;
                fromAddr = Properties.Current.SchedulerEmail;
                toAddr = rsv.Client.ActiveEmails();
                subject = string.Format("{0} - Reservation Created", Providers.Email.CompanyName);
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been created.",
                    rsv.Client.DisplayName,
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.Resource.ResourceName,
                    Environment.NewLine);

                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnUserCreate(Reservation rsv)", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when his/her reservation is updated, either by reserver or by TE
        public static void EmailOnUserUpdate(Reservation rsv)
        {
            ClientSetting reserverSetting = DA.Current.Single<ClientSetting>(rsv.Client.ClientID);
            if (reserverSetting.EmailModifyReserv.Value || rsv.Client.ClientID != CacheManager.Current.CurrentUser.ClientID)
            {
                string fromAddr, subject, body;
                fromAddr = Properties.Current.SchedulerEmail;
                IEnumerable<string> toAddr = rsv.Client.ActiveEmails();
                subject = string.Format("{0} - Reservation Updated", Providers.Email.CompanyName);
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been updated.",
                    rsv.Client.DisplayName,
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.Resource.ResourceName,
                    Environment.NewLine);

                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnUserUpdate(Reservation rsv)", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when his/her reservation is deleted, either by reserver or by TE
        public static void EmailOnUserDelete(Reservation rsv)
        {
            ClientSetting reserverSetting = DA.Current.Single<ClientSetting>(rsv.Client.ClientID);
            if (reserverSetting.EmailDeleteReserv.Value || rsv.Client.ClientID != CacheManager.Current.CurrentUser.ClientID)
            {
                string fromAddr, subject, body;
                fromAddr = Properties.Current.SchedulerEmail;
                IEnumerable<string> toAddr = rsv.Client.ActiveEmails();
                subject = string.Format("{0} - Reservation Deleted", Providers.Email.CompanyName);
                body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been deleted.",
                    rsv.Client.DisplayName,
                    rsv.BeginDateTime.ToString(Reservation.DateFormat),
                    rsv.EndDateTime.ToString(Reservation.DateFormat),
                    rsv.Resource.ResourceName,
                    Environment.NewLine);

                Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnUserDelete(Reservation rsv)", subject, body, fromAddr, toAddr);
            }
        }

        //Send email to reserver when reservation is overrode by TE
        public static void EmailOnToolEngDelete(Reservation rsv, int toolEngClientId)
        {
            Client toolEng = DA.Current.Single<Client>(toolEngClientId);

            string fromAddr, subject, body;
            fromAddr = toolEng.PrimaryEmail();
            IEnumerable<string> toAddr = rsv.Client.ActiveEmails();
            subject = string.Format("{0} - Reservation for {1} has been removed", Providers.Email.CompanyName, rsv.Client.DisplayName);
            body = string.Format("{0}{7}{7}Your reservation beginning at {1} and ending at {2} for resource {3} has been removed because {4}, who has administrator or tool engineer access, has made a reservation that overrides yours.{7}{7}If you have any questions please contact {4} at {5}.{7}{7}Notes from reserver:{7}{6}",
                rsv.Client.DisplayName,
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.Resource.ResourceName,
                toolEng.DisplayName,
                fromAddr,
                rsv.Notes,
                Environment.NewLine);

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnToolEngDelete(Reservation rsv, int toolEngClientId)", subject, body, fromAddr, toAddr);
        }

        public enum ReservationModificationType
        {
            Created = 1,
            Modified = 2
        }

        //Send email to invitees when they are invited to a reservation
        public static void EmailOnInvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees, ReservationModificationType modificationType = ReservationModificationType.Created)
        {
            if (invitees == null || invitees.Count() == 0) return;
            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = string.Format("{0} - Reservation Invitation", Providers.Email.CompanyName);

            //reservationType
            string invitedModifiedText;

            if (modificationType == ReservationModificationType.Created)
                invitedModifiedText = "invited you to a reservation on";
            else
                invitedModifiedText = "modified a reservation that you are invited to";

            body = string.Format("{0} has {1} {2} for resource {3}.", rsv.Client.DisplayName, invitedModifiedText, rsv.BeginDateTime.ToString(Reservation.DateFormat), rsv.Resource.ResourceName);

            foreach (var ri in invitees)
            {
                //Send email if invitee wants to receive email
                ClientSetting inviteeSetting = DA.Current.Single<ClientSetting>(ri.InviteeID);
                if (inviteeSetting != null)
                {
                    if (inviteeSetting.EmailInvited.Value)
                    {
                        var primary = ClientOrgUtility.GetPrimary(ri.InviteeID);
                        if (primary != null)
                            toAddr.Add(primary.Email);
                    }
                }
            }

            if (toAddr.Count == 0) return;

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnInvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees, ReservationModificationType modificationType = ReservationModificationType.Created)", subject, body, fromAddr, toAddr);
        }

        //Send email to invitees when they are uninvited to a reservation
        public static void EmailOnUninvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees)
        {
            if (invitees == null || invitees.Count() == 0) return;
            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();

            fromAddr = Properties.Current.SchedulerEmail;
            subject = string.Format("{0} - Reservation Invitation Canceled", Providers.Email.CompanyName);
            body = string.Format("Your invitation to a reservation made by {0} on {1} for resource {2} has been canceled.",
                rsv.Client.DisplayName,
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.Resource.ResourceName);

            foreach (var ri in invitees)
            {
                //Send email if invitee wants to receive email
                ClientSetting inviteeSetting = DA.Current.Single<ClientSetting>(ri.InviteeID);
                if (inviteeSetting != null && inviteeSetting.IsValid())
                {
                    if (inviteeSetting.EmailInvited.Value)
                    {
                        var primary = ClientOrgUtility.GetPrimary(ri.InviteeID);
                        if (primary != null)
                            toAddr.Add(primary.Email);
                    }
                }
            }

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnUninvited(Reservation rsv, IEnumerable<ReservationInviteeItem> invitees)", subject, body, fromAddr, toAddr);
        }

        //Email users who want to be notified of open reservation slots
        public static void EmailOnOpenSlot(int resourceId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int reservationId)
        {
            ResourceModel res = CacheManager.Current.GetResource(resourceId);

            IList<ResourceClientInfo> clients = null;
            string footer = string.Empty;

            if (notifyType == EmailNotify.Always)
            {
                clients = ResourceClientInfoUtility.SelectNotifyOnCancelClients(res.ResourceID).ToList();
                footer = string.Format("Sent to NotifyOnCancel clients, ReservationID: {0}", reservationId);
            }
            else if (notifyType == EmailNotify.OnOpening)
            {
                clients = ResourceClientInfoUtility.SelectNotifyOnOpeningClients(res.ResourceID).ToList();
                footer = string.Format("Sent to NotifyOnOpening clients, ReservationID: {0}", reservationId);
            }

            if (clients == null || clients.Count == 0) return;

            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = string.Format("{0} - Open reservation slot for {1}", Providers.Email.CompanyName, res.ResourceName);
            body = string.Format("{0} just became available for reservation from {1} to {2}.{3}{3}If you wish to reserve this resource, please sign up quickly.",
                res.ResourceName,
                beginDateTime.ToString(Reservation.DateFormat),
                endDateTime.ToString(Reservation.DateFormat),
                Environment.NewLine
            );

            foreach (ResourceClientInfo rc in clients)
            {
                if (!rc.IsEveryone())
                    toAddr.Add(rc.Email);
            }

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnOpenSlot(int resourceId, DateTime beginDateTime, DateTime endDateTime, EmailNotify notifyType, int reservationId)", subject, body, fromAddr, toAddr);
        }

        public static void EmailOnPracticeRes(Reservation rsv, string inviteeName)
        {
            IList<ResourceClientInfo> clients = ResourceClientInfoUtility.SelectNotifyOnPracticeRes(rsv.Resource.ResourceID).ToList();
            if (clients == null || clients.Count == 0) return;

            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            fromAddr = Properties.Current.SchedulerEmail;
            subject = string.Format("{0} - {1} has made Practice reservation on {2}", Providers.Email.CompanyName, rsv.Client.DisplayName, rsv.Resource.ResourceName);
            body = string.Format("Practice reservation from {0} to {1}. The invitee is {2}.",
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                inviteeName);

            foreach (ResourceClientInfo rc in clients)
            {
                if (!rc.IsEveryone())
                    toAddr.Add(rc.Email);
            }

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnPracticeRes(Reservation rsv, string inviteeName)", subject, body, fromAddr, toAddr);
        }

        //Email reservers when their reservations are canceled because TE changed granularity.
        public static void EmailOnCanceledByResource(Reservation rsv)
        {
            string fromAddr, subject, body;
            fromAddr = Properties.Current.SchedulerEmail;
            IEnumerable<string> toAddr = rsv.Client.ActiveEmails();

            subject = string.Format("{0} - Reservation Canceled", Providers.Email.CompanyName);
            body = string.Format("{0}{4}{4}Your reservation beginning at {1} and ending at {2} for resource {3} has been canceled due to an update in the resource configuration.{4}{4}If you have any questions please contact the tool engineer.",
                rsv.Client.DisplayName,
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.Resource.ResourceName,
                Environment.NewLine);

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnCanceledByResource(Reservation rsv)", subject, body, fromAddr, toAddr);
        }

        //Email reservers when their reservations are canceled because TE needs to repair resource.
        public static EmailResult EmailOnCanceledByRepair(Reservation rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime)
        {
            string fromAddr, subject, body;
            fromAddr = Properties.Current.SchedulerEmail;
            IEnumerable<string> toAddr = rsv.Client.ActiveEmails();

            subject = string.Format("{0} - Reservation Canceled", Providers.Email.CompanyName);
            body = string.Format("{0}{8}{8}Your reservation beginning at {1} and ending at {2} for resource {3} has been {4} because this resource has been marked '{5}' until {6}.{8}{8}The reason for the change:{8}{7}{8}{8}If you have any questions, please contact the tool engineer.",
                rsv.Client.DisplayName,
                rsv.BeginDateTime.ToString(Reservation.DateFormat),
                rsv.EndDateTime.ToString(Reservation.DateFormat),
                rsv.Resource.ResourceName,
                (isRemoved) ? "removed" : "forced to end",
                state,
                repairEndDateTime.ToString(Reservation.DateFormat),
                notes,
                Environment.NewLine);

            var sendResult = Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnCanceledByRepair(Reservation rsv, bool isRemoved, string state, string notes, DateTime repairEndDateTime)", subject, body, fromAddr, toAddr);

            return new EmailResult(sendResult.Success, sendResult.GetErrorMessage(), subject, body, fromAddr, toAddr);
        }

        //Email reservers when tool engineers have forgiven charges to their reservations
        public static void EmailOnForgiveCharge(Reservation rsv, double forgiveAmount, bool sendToUser)
        {
            string fromAddr, subject, body;
            List<string> toAddr = new List<string>();
            string emailForgivenCharge = ConfigurationManager.AppSettings["EmailForgivenCharge"];
            //TimeSpan ts = ReservationUtility.GetEndDateTime(rsv) - ReservationUtility.GetBeginDateTime(rsv);
            TimeSpan ts = rsv.ChargeEndDateTime() - rsv.ChargeBeginDateTime();
            fromAddr = Properties.Current.SchedulerEmail;

            if (sendToUser) toAddr.AddRange(rsv.Client.ActiveEmails());

            if (!string.IsNullOrEmpty(emailForgivenCharge))
                toAddr.Add(emailForgivenCharge);

            if (toAddr.Count == 0) return;

            subject = string.Format("{0} - Reservation Charges Forgiven", Providers.Email.CompanyName);
            body = string.Format("This is an automatically generated email to let you know that {0}% of the charges on run number {1} have been forgiven by one of the tool engineers.<br /><br />Reservation Detail:<ul><li>Tool: {2}</li><li>Chargeable Time: from {3} to {4}</li><li>Total Chargeable Minutes: {5}</li></ul>",
                forgiveAmount,
                rsv.ReservationID,
                rsv.Resource.ResourceName,
                rsv.ChargeBeginDateTime().ToString(Reservation.DateFormat),
                rsv.ChargeEndDateTime().ToString(Reservation.DateFormat),
                //ReservationUtility.GetBeginDateTime(rsv).ToString(ReservationUtility.DateFormat),
                //ReservationUtility.GetEndDateTime(rsv).ToString(ReservationUtility.DateFormat),
                ts.TotalMinutes.ToString("0.##"));

            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailOnForgiveCharge(Reservation rsv, double forgiveAmount, bool sendToUser)", subject, body, fromAddr, toAddr, isHtml: true);
        }

        public static void EmailFromUser(string toAddr, string subject, string body, bool ccself = false, bool isHtml = false)
        {
            string email = CacheManager.Current.Email;
            List<string> cc = new List<string>();
            if (ccself) cc.Add(email);
            Providers.Email.SendMessage(CacheManager.Current.ClientID, "LNF.Scheduler.EmailUtility.EmailFromUser(string toAddr, string subject, string body, bool ccself = false, bool isHtml = false)", subject, body, email, new string[] { toAddr }, cc: cc, isHtml: isHtml);
        }
    }

    public struct EmailResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        public string Subject { get; }
        public string Body { get; }
        public string FromAddress { get; }
        public IEnumerable<string> ToAddresses { get; }

        public EmailResult(bool success, string errmsg, string subj, string body, string fromAddr, IEnumerable<string> toAddrs)
        {
            Success = success;
            ErrorMessage = errmsg;
            Subject = subj;
            Body = body;
            FromAddress = fromAddr;
            ToAddresses = toAddrs;
        }
    }

}
