using LNF.Data;
using LNF.Mail;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;

namespace LNF.PhysicalAccess
{
    public class RoomAccessExpirationCheck
    {
        public int Run()
        {
            var dataFeed = GetDataFeed();
            return EmailExpiringCards(dataFeed.Data.Items(new ExpiringCardConverter()));
        }

        private DateTime GetMinDateTime(DateTime d1, DateTime d2)
        {
            if (d1 < d2)
                return d1;
            else
                return d2;
        }

        private int EmailExpiringCards(IEnumerable<ExpiringCard> data)
        {
            int count = 0;

            foreach (var item in data)
            {
                if (!item.Email.StartsWith("none"))
                {
                    try
                    {
                        // per Sandrine as of 2017-10-02 [jg]
                        string bodyTemplate = "Dear {1} {2},{0}{0}You are receiving this email because your safety training - and therefore your LNF access - will expire on {3:M/d/yyyy}. In order to ensure continuous lab access, please complete the online training by following the instructions in step 5 (new user training) of the following page:{0}- for internal users: {4}{0}- for external academic users: {5}{0}- for external non-academic users: {6}{0}{0}The system will ask you to login with your LNF Online Services credentials before completing the safety training.{0}{0}If you no longer need access to the LNF, or if you have any question, please let us know.{0}{0}Thank you,{0}LNF Staff";

                        var links = new
                        {
                            internalUsers = "https://LNF.umich.edu/getting-access/internal-users/",
                            externalAcademicUsers = "https://LNF.umich.edu/getting-access/external-academic-users/",
                            nonAcademicUsers = "https://LNF.umich.edu/getting-access/non-academic-users/"
                        };

                        var expireDate = GetMinDateTime(item.CardExpireDate, item.BadgeExpireDate);

                        var addr = new MailAddress(item.Email);
                        string from = "lnf-access@umich.edu";
                        string subj = "LNF Access Card Expiring Soon";
                        string body = string.Format(bodyTemplate,
                            Environment.NewLine,
                            item.FName,
                            item.LName,
                            expireDate,
                            links.internalUsers,
                            links.externalAcademicUsers,
                            links.nonAcademicUsers);

                        string[] cc = GetExpiringCardsEmailRecipients();

                        ServiceProvider.Current.Mail.SendMessage(new SendMessageArgs
                        {
                            ClientID = 0,
                            Caller = "LNF.Scheduler.Service.Process.CheckClientIssues.EmailExpiringCards",
                            Subject = subj,
                            Body = body,
                            From = from,
                            To = new[] { item.Email },
                            Cc = cc
                        });

                        count++;
                    }
                    catch
                    {
                        //invalid address so skip it
                    }
                }
            }

            return count;
        }

        public DataFeedResult GetDataFeed()
        {
            var result = ServiceProvider.Current.Data.Feed.GetDataFeedResult("expiring-cards");
            return result;
        }

        private string[] GetExpiringCardsEmailRecipients()
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExpiringCardsEmailRecipients"]))
                return null;
            else
                return ConfigurationManager.AppSettings["ExpiringCardsEmailRecipients"].Split(',');
        }
    }
}