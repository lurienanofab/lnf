using LNF.Scheduler;
using System.Linq;

namespace LNF.Impl.Repository.Scheduler
{
    public static class Extensions
    {
        public static IOrderedQueryable<IResource> DefaultResourceOrderBy(this IQueryable<IResource> query)
        {
            return query
                .OrderBy(x => x.BuildingName)
                .ThenBy(x => x.LabName)
                .ThenBy(x => x.ProcessTechName)
                .ThenBy(x => x.ResourceName);
        }
    }
}
