using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ResourceClientInfoUtility
    {
        public static IList<ResourceClientInfo> Select(int? resourceId = null, int? clientId = null)
        {
            IQueryable<ResourceClientInfo> query;

            IList<ResourceClientInfo> result;

            if (resourceId.HasValue)
                query = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId.Value);
            else
                query = DA.Current.Query<ResourceClientInfo>();

            if (clientId.HasValue)
                result = query.Where(x => x.ClientID == clientId.Value || x.ClientID == -1).ToList();
            else
                result = query.ToList();

            return result;
        }

        public static IQueryable<ResourceClientInfo> GetResourceClients(int resourceId)
        {
            var result = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId);
            return result;
        }

        public static IQueryable<ResourceClientInfo> GetToolEngineers(int resourceId)
        {
            return GetResourceClients(resourceId).Where(x => (x.AuthLevel & ClientAuthLevel.ToolEngineer) > 0 && (x.Expiration == null || x.Expiration.Value > DateTime.Now));
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(int resourceId)
        {
            return DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId && x.AuthLevel > ClientAuthLevel.UnauthorizedUser && (x.EmailNotify != null && x.EmailNotify.Value == 1));
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(int resourceId)
        {
            return DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId && x.AuthLevel > ClientAuthLevel.UnauthorizedUser && (x.EmailNotify != null && x.EmailNotify.Value == 2));
        }

        public static IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(int resourceId)
        {
            return DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId && x.AuthLevel == ClientAuthLevel.ToolEngineer && (x.PracticeResEmailNotify != null && x.PracticeResEmailNotify.Value == 1));
        }
    }
}
