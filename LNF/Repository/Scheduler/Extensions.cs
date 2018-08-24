using LNF.Models.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public static class Extensions
    {
        public static ResourceClientItem GetResourceClientItem(this ResourceClientInfo item)
        {
            if (item == null) return null;
            return new ResourceClientItem()
            {
                ResourceClientID = item.ResourceClientID,
                ResourceID = item.ResourceID,
                ResourceName = item.ResourceName,
                ClientID = item.ClientID,
                UserName = item.UserName,
                DisplayName = item.DisplayName,
                Privs = item.Privs,
                AuthLevel = item.AuthLevel,
                Expiration = item.Expiration,
                EmailNotify = item.EmailNotify,
                PracticeResEmailNotify = item.PracticeResEmailNotify,
                Email = item.Email,
                ClientActive = item.ClientActive
            };
        }

        public static IEnumerable<ResourceClientItem> GetResourceClientItems(this IQueryable<ResourceClientInfo> query)
        {
            return query.Select(x => new ResourceClientItem()
            {
                ResourceClientID = x.ResourceClientID,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                ClientID = x.ClientID,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                Privs = x.Privs,
                AuthLevel = x.AuthLevel,
                Expiration = x.Expiration,
                EmailNotify = x.EmailNotify,
                PracticeResEmailNotify = x.PracticeResEmailNotify,
                Email = x.Email,
                ClientActive = x.ClientActive
            });
        }
    }
}
