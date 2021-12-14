using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ResourceClients
    {
        public static IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            return ServiceProvider.Current.Scheduler.Resource.GetResourceClients(resourceId);
        }

        public static IEnumerable<IResourceClient> GetToolEngineers(int resourceId)
        {
            return ServiceProvider.Current.Scheduler.Resource.GetActiveResourceClients(resourceId, authLevel: ClientAuthLevel.ToolEngineer);
        }

        public static IEnumerable<IResourceClient> SelectNotifyOnCancelClients(int resourceId)
        {
            return SelectEmailClients(resourceId).Where(x => x.AuthLevel > ClientAuthLevel.UnauthorizedUser && x.EmailNotify.Value == 1).ToList();
        }

        public static IEnumerable<IResourceClient> SelectNotifyOnOpeningClients(int resourceId)
        {
            return SelectEmailClients(resourceId).Where(x => x.AuthLevel > ClientAuthLevel.UnauthorizedUser && x.EmailNotify.Value == 2).ToList();
        }

        public static IEnumerable<IResourceClient> SelectNotifyOnPracticeRes(int resourceId)
        {
            return GetResourceClients(resourceId).Where(x => x.AuthLevel == ClientAuthLevel.ToolEngineer && (x.PracticeResEmailNotify != null && x.PracticeResEmailNotify.Value == 1));
        }

        /// <summary>
        /// Returns all clients who want to receive email notification for the speicified resource.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectEmailClients(int resourceId)
        {
            return GetResourceClients(resourceId).Where(x => x.EmailNotify != null).ToList();
        }

        /// <summary>
        /// Retrieves auths for a specified client.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectByClient(int clientId)
        {
            return ServiceProvider.Current.Scheduler.Resource.GetResourceClients(clientId: clientId);
        }

        /// <summary>
        /// Retrieves auths for a specified resource.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectByResource(int resourceId)
        {
            return ServiceProvider.Current.Scheduler.Resource.GetResourceClients(resourceId).Where(x => x.ClientActive).ToList();
        }

        public static bool HasAuth(ClientAuthLevel auth1, ClientAuthLevel auth2) => (auth1 & auth2) > 0;

        public static bool IsEveryone(int clientId) => clientId == -1;

        public static IEnumerable<IResourceClient> SelectByResource(int resourceId, ClientAuthLevel authLevel)
        {
            return ServiceProvider.Current.Scheduler.Resource
                .GetResourceClients(resourceId, authLevel: authLevel)
                .Where(x => x.ClientActive).ToList();
        }

        /// <summary>
        /// Returns all clients whose authorization is about to expire.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectExpiringClients()
        {
            return ServiceProvider.Current.Scheduler.Resource.GetExpiringResourceClients();
        }

        /// <summary>
        /// Returns resources with Everyone authorization that is about to expire.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectExpiringEveryone()
        {
            return ServiceProvider.Current.Scheduler.Resource.GetExpiringResourceClients(true);
        }

        /// <summary>
        /// Returns all clients whose authorization has expired.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectExpiredClients()
        {
            return ServiceProvider.Current.Scheduler.Resource.GetExpiredResourceClients();
        }

        /// <summary>
        /// Returns resources with Everyone authorization that has expired.
        /// </summary>
        public static IEnumerable<IResourceClient> SelectExpiredEveryone()
        {
            return ServiceProvider.Current.Scheduler.Resource.GetExpiredResourceClients(true);
        }

        /// <summary>
        /// Deletes all clients whose authorization has expired.
        /// </summary>
        public static int DeleteExpiredClients()
        {
            return ServiceProvider.Current.Scheduler.Resource.DeleteExpiredResourceClients();
        }

        /// <summary>
        /// Check for authorized clients whose authorizations are about to expire and email user. NOTE: Must deal with everyone separate from others.
        /// </summary>
        public static CheckExpiringClientsProcessResult CheckExpiringClients(IEnumerable<IResourceClient> expiringClients, IEnumerable<IResourceClient> expiringEveryone, bool noEmail = false)
        {
            //send warning to users with authorizations about to expire
            var result = new CheckExpiringClientsProcessResult()
            {
                ExpiringClientsCount = expiringClients.Count(),
                ExpiringEveryoneCount = expiringEveryone.Count()
            };

            foreach (IResourceClient item in expiringClients)
            {
                //only send warnings to client who are still active
                if (item.ClientActive && item.Expiration != null)
                {
                    //Check that the current time is within 5 min past the warning time
                    //To prevent repeated sending of emails
                    string resourceName = item.ResourceName;
                    var expiration = item.Expiration.Value;
                    var warning = GetWarningDate(item).Value;

                    //Email client
                    double daysTillExpire = DateTime.Now.Subtract(warning).TotalDays;
                    if (daysTillExpire > 0 && daysTillExpire <= 1)
                    {
                        string recipient = item.Email;
                        string subject = $"{SendEmail.CompanyName} tool authorization is about to expire!";
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
            foreach (IResourceClient item in expiringEveryone)
            {
                if (item.Expiration != null)
                {
                    //Check that the current time is within 5 min past the warning time
                    //To prevent repeated sending of emails
                    string resourceName = item.ResourceName;
                    DateTime expiration = item.Expiration.Value;
                    DateTime warning = GetWarningDate(item).Value;

                    //Email client
                    double daysTillExpire = DateTime.Now.Subtract(warning).TotalDays;
                    if (daysTillExpire > 0 && daysTillExpire <= 1)
                    {
                        List<string> recipient = new List<string>();
                        string subject = string.Empty;

                        IList<IResourceClient> engineers = GetToolEngineers(item.ResourceID).ToList();
                        recipient.AddRange(engineers.Select(x => x.Email));
                        subject = $"{SendEmail.CompanyName} tool authorization is about to expire!";

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
        public static CheckExpiredClientsProcessResult CheckExpiredClients(IEnumerable<IResourceClient> expiredClients, IEnumerable<IResourceClient> expiredEveryone, bool noEmail = false)
        {
            //send notice to users with authorizations that have expired
            var result = new CheckExpiredClientsProcessResult()
            {
                ExpiredClientsCount = expiredClients.Count(),
                ExpiredEveryoneCount = expiredEveryone.Count()
            };

            foreach (IResourceClient item in expiredClients)
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
            foreach (IResourceClient item in expiredEveryone)
            {
                if (item.Expiration != null)
                {
                    //only send warnings to client who are still active
                    List<string> recipient = new List<string>();
                    string subject = string.Empty;

                    IList<IResourceClient> engineers = GetToolEngineers(item.ResourceID).ToList();
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
            IList<IResourceClient> engineers = GetToolEngineers(resourceId).ToList();
            string result = string.Join("; ", engineers.Select(x => x.DisplayName + " - " + x.Email));
            return result;
        }

        public static string[] SelectEmailsByAuth(int resourceId, ClientAuthLevel auth)
        {
            return ServiceProvider.Current.Scheduler.Resource
                .GetResourceClients(resourceId, authLevel: auth)
                .Where(x => !x.IsEveryone())
                .Select(x => x.Email)
                .ToArray();
        }

        public static DateTime? GetWarningDate(IResourceClient rc)
        {
            var authExpWarning = Properties.Current.AuthExpWarning;
            if (!rc.Expiration.HasValue) return null;
            DateTime result = rc.Expiration.Value.AddDays(-30 * authExpWarning * rc.AuthDuration);
            return result;
        }
    }
}
