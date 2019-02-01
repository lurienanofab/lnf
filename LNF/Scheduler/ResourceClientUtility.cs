using LNF.Cache;
using LNF.CommonTools;
using LNF.Logging;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ResourceClientUtility
    {
        /// <summary>
        /// Returns all clients who want to receive email notification for the speicified resource.
        /// </summary>
        public static IQueryable<ResourceClient> SelectEmailClients(int resourceId)
        {
            return DA.Current.Query<ResourceClient>().Where(x => x.Resource.ResourceID == resourceId && x.EmailNotify != null && x.EmailNotify.Value != 0);
        }

        /// <summary>
        /// Retrieves auths for a specified client.
        /// </summary>
        public static IQueryable<ResourceClientInfo> SelectByClient(int clientId)
        {
            IQueryable<ResourceClientInfo> query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID == clientId || x.ClientID == -1);
            if (query == null) return null;
            return query.Where(x => x.ClientActive);
        }

        /// <summary>
        /// Retrieves auths for a specified resource.
        /// </summary>
        public static IQueryable<ResourceClientInfo> SelectByResource(int resourceId)
        {
            IQueryable<ResourceClientInfo> result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId && x.ClientActive);
            return result;
        }

        public static IQueryable<ResourceClientInfo> SelectByResource(int resourceId, ClientAuthLevel authLevel)
        {
            IQueryable<ResourceClientInfo> result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId && (x.AuthLevel & authLevel) > 0 && x.ClientActive);
            return result;
        }

        /// <summary>
        /// Returns all clients whose authorization is about to expire.
        /// </summary>
        public static IList<ResourceClientInfo> SelectExpiringClients()
        {
            DateTime now = DateTime.Now;
            var query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID != -1 && x.AuthLevel == ClientAuthLevel.AuthorizedUser && x.Expiration != null).ToList();
            var result = query.Where(x => now > x.WarningDate(Properties.Current.AuthExpWarning)).ToList();
            return result;
        }

        /// <summary>
        /// Returns resources with Everyone authorization that is about to expire.
        /// </summary>
        public static IList<ResourceClientInfo> SelectExpiringEveryone()
        {
            var query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID == -1 && x.AuthLevel == ClientAuthLevel.AuthorizedUser && x.Expiration != null).ToArray();
            var result = query.Where(x => DateTime.Now > x.WarningDate(Properties.Current.AuthExpWarning)).ToList();
            return result;
        }

        /// <summary>
        /// Returns all clients whose authorization has expired.
        /// </summary>
        public static IList<ResourceClientInfo> SelectExpiredClients()
        {
            return DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID != -1 && x.AuthLevel <= ClientAuthLevel.AuthorizedUser).ToArray()
                .Where(x => x.Expiration != null && x.Expiration.Value < DateTime.Now && x.ResourceIsActive)
                .ToList();
        }

        /// <summary>
        /// Returns resources with Everyone authorization that has expired.
        /// </summary>
        public static IList<ResourceClientInfo> SelectExpiredEveryone()
        {
            return DA.Current.Query<ResourceClientInfo>().Where(x => x.ClientID == -1 && x.AuthLevel <= ClientAuthLevel.AuthorizedUser).ToList()
                .Where(x => x.Expiration != null && x.Expiration.Value < DateTime.Now && x.ResourceIsActive)
                .ToList();
        }

        /// <summary>
        /// Deletes all clients whose authorization has expired.
        /// </summary>
        public static int DeleteExpiredClients()
        {
            int result = 0;
            IList<ResourceClient> expiredClients = DA.Current.Query<ResourceClient>().Where(x => x.AuthLevel <= ClientAuthLevel.AuthorizedUser && x.Expiration != null && x.Expiration.Value < DateTime.Now).ToList();
            result = expiredClients.Count;
            DA.Current.Delete(expiredClients);
            return result;
        }

        /// <summary>
        /// Check for authorized clients whose authorizations are about to expire and email user. NOTE: Must deal with everyone separate from others.
        /// </summary>
        public static CheckExpiringClientsProcessResult CheckExpiringClients(IEnumerable<ResourceClientInfo> expiringClients, IEnumerable<ResourceClientInfo> expiringEveryone, bool noEmail = false)
        {
            //send warning to users with authorizations about to expire
            var result = new CheckExpiringClientsProcessResult()
            {
                ExpiringClientsCount = expiringClients.Count(),
                ExpiringEveryoneCount = expiringEveryone.Count()
            };

            foreach (ResourceClientInfo item in expiringClients)
            {
                //only send warnings to client who are still active
                if (item.ClientActive && item.Expiration != null)
                {
                    //Check that the current time is within 5 min past the warning time
                    //To prevent repeated sending of emails
                    string resourceName = item.ResourceName;
                    DateTime expiration = item.Expiration.Value;
                    DateTime warning = item.WarningDate(Properties.Current.AuthExpWarning).Value;

                    //Email client
                    double daysTillExpire = DateTime.Now.Subtract(warning).TotalDays;
                    if (daysTillExpire > 0 && daysTillExpire <= 1)
                    {
                        string recipient = item.Email;
                        string subject = "LNF tool authorization is about to expire!";
                        string body = "Your authorization for the " + resourceName
                            + string.Format(" will expire on {0}.<br /><br />", expiration.ToLongDateString())
                            + string.Format("Please <a href=\"{0}\">create a ticket on the tool</a>", HelpdeskUtility.GetSchedulerHelpdeskUrl("ssel-sched.eecs.umich.edu", item.ResourceID))
                            + " to extend your authorization for the tool.";

                        try
                        {
                            if (!string.IsNullOrEmpty(recipient))
                            {
                                if (!noEmail)
                                    SendEmail.SendSystemEmail("LNF.Scheduler.ResourceClientUtility.CheckExpiringClients", subject, body, new[] { recipient });

                                result.Data.Add($"Expiring authorization: {string.Join(",", recipient)}, Resource: {item.ResourceName} [{item.ResourceID}]");
                                result.ExpiringClientsEmailsSent += 1;
                            }
                            else
                                result.Data.Add($"Expiring authorization: No recipient, Resource: {item.ResourceName} [{item.ResourceID}]");
                        }
                        catch (Exception ex)
                        {
                            result.Data.Add($"Expiring authorization ERROR: {recipient}, Resource: {item.ResourceName} [{item.ResourceID}], Message: {ex.Message}");
                        }
                    }
                }
            }

            //send warning to tool eng who manage everyones about to expire
            //For each client whose authorization is about to expire
            foreach (ResourceClientInfo item in expiringEveryone)
            {
                if (item.Expiration != null)
                {
                    //Check that the current time is within 5 min past the warning time
                    //To prevent repeated sending of emails
                    string resourceName = item.ResourceName;
                    DateTime expiration = item.Expiration.Value;
                    DateTime warning = item.WarningDate(Properties.Current.AuthExpWarning).Value;

                    //Email client
                    double daysTillExpire = DateTime.Now.Subtract(warning).TotalDays;
                    if (daysTillExpire > 0 && daysTillExpire <= 1)
                    {
                        List<string> recipient = new List<string>();
                        string subject = string.Empty;

                        IList<ResourceClientInfo> engineers = ResourceClientInfoUtility.GetToolEngineers(item.ResourceID).ToList();
                        recipient.AddRange(engineers.Select(x => x.Email));
                        subject = "LNF tool authorization is about to expire!";

                        string body = "Everyone authorization for the " + resourceName
                            + " will expire on " + expiration.ToLongDateString() + ".<br /><br />"
                            + "Please update the authorization for the tool as appropriate.";

                        try
                        {
                            if (recipient.Count > 0)
                            {
                                if (!noEmail)
                                    SendEmail.SendSystemEmail("LNF.Scheduler.ResourceClientUtility.CheckExpiringClients", subject, body, recipient);

                                result.Data.Add($"Expiring Everyone authorization: {string.Join(",", recipient)}, Resource: {item.ResourceName} [{item.ResourceID}]");
                                result.ExpiringEveryoneEmailsSent += 1;
                            }
                            else
                                result.Data.Add($"Expiring Everyone authorization: No recipient, Resource: {item.ResourceName} [{item.ResourceID}]");
                        }
                        catch (Exception ex)
                        {
                            result.Data.Add($"Expiring Everyone authorization ERROR: {string.Join(",", recipient)}, Resource: {item.ResourceName} [{item.ResourceID}], Message: {ex.Message}");
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check for clients whose authorization has expired.
        /// </summary>
        public static CheckExpiredClientsProcessResult CheckExpiredClients(IEnumerable<ResourceClientInfo> expiredClients, IEnumerable<ResourceClientInfo> expiredEveryone, bool noEmail = false)
        {
            //send notice to users with authorizations that have expired
            var result = new CheckExpiredClientsProcessResult()
            {
                ExpiredClientsCount = expiredClients.Count(),
                ExpiredEveryoneCount = expiredEveryone.Count()
            };

            foreach (ResourceClientInfo item in expiredClients)
            {
                //only send warnings to client who are still active
                if (item.ClientActive && item.Expiration != null)
                {
                    string recipient = item.Email;
                    string subject = "Your authorization has expired!";
                    string body = string.Format("Your authorization for the {0}", item.ResourceName)
                        + string.Format(" expired on {0}.<br /><br />", item.Expiration.Value.ToLongDateString())
                        + string.Format("Please <a href=\"{0}\">create a ticket on the tool</a>", HelpdeskUtility.GetSchedulerHelpdeskUrl("ssel-sched.eecs.umich.edu", item.ResourceID))
                        + " to extend your authorization for the tool.";

                    try
                    {
                        if (recipient.Count() > 0)
                        {
                            if (!noEmail)
                                SendEmail.SendSystemEmail("LNF.Scheduler.ResourceClientUtility.CheckExpiredClients", subject, body, new[] { recipient });

                            result.Data.Add($"Expired authorization: {recipient}, Resource: {item.ResourceName} [{item.ResourceID}]");
                            result.ExpiredClientsEmailsSent += 1;
                        }
                        else
                            result.Data.Add($"Expired authorization: No recipient, Resource: {item.ResourceName} [{item.ResourceID}]");
                    }
                    catch (Exception ex)
                    {
                        result.Data.Add($"Expired authorization ERROR: {recipient}, Resource: {item.ResourceName} [{item.ResourceID}], Message: {ex.Message}");
                    }
                }
            }

            //send notice to tool eng who manage everyones that have expired
            foreach (ResourceClientInfo item in expiredEveryone)
            {
                if (item.Expiration != null)
                {
                    //only send warnings to client who are still active
                    List<string> recipient = new List<string>();
                    string subject = string.Empty;

                    IList<ResourceClientInfo> engineers = ResourceClientInfoUtility.GetToolEngineers(item.ResourceID).ToList();
                    recipient.AddRange(engineers.Select(x => x.Email));
                    subject = "Everyone authorization has expired!";

                    string body = "The Everyone authorization for the " + item.ResourceName
                        + " expired on " + item.Expiration.Value.ToLongDateString() + ".<br /><br />"
                        + "Please update the authorization for the tool as appropriate.";

                    try
                    {
                        if (recipient.Count > 0)
                        {
                            if (!noEmail)
                                SendEmail.SendSystemEmail("LNF.Scheduler.ResourceClientUtility.CheckExpiredClients", subject, body, recipient);

                            result.Data.Add($"Expired Everyone authorization: {string.Join(",", recipient)}, Resource: {item.ResourceName} [{item.ResourceID}]");
                            result.ExpiredEveryoneEmailsSent += 1;
                        }
                        else
                            result.Data.Add($"Expired Everyone authorization: No recipient, Resource: {item.ResourceName} [{item.ResourceID}]");
                    }
                    catch (Exception ex)
                    {
                        result.Data.Add($"Expired Everyone authorization ERROR: {string.Join(",", recipient)}, Resource: {item.ResourceName} [{item.ResourceID}], Message: {ex.Message}");
                    }
                }
            }

            //Delete Clients
            result.DeleteExpiredClientsCount = DeleteExpiredClients();

            return result;
        }

        /// <summary>
        /// Retrieve a list of tool engineer names and emails for a specified resource.
        /// </summary>
        public static string ToolEngineerList(int resourceId)
        {
            IList<ResourceClientInfo> engineers = ResourceClientInfoUtility.GetToolEngineers(resourceId).ToList();
            string result = string.Join("; ", engineers.Select(x => x.DisplayName + " - " + x.Email));
            return result;
        }

        public static string[] SelectEmailsByAuth(int resourceId, ClientAuthLevel auth)
        {
            string[] result = DA.Current.Query<ResourceClientInfo>()
                .Where(x => x.ResourceID == resourceId).ToList()
                .Where(x => (x.AuthLevel & auth) > 0 && !x.IsEveryone())
                .Select(x => x.Email)
                .ToArray();
            return result;
        }
    }
}
