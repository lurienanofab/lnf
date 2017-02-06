using LNF.Repository.Scheduler;
using NHibernate.Context;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class ResourceRepository<TContext> : Repository<TContext, Resource>, IResourceRepository
        where TContext : ICurrentSessionContext
    {
        public IList<Resource> SelectActive()
        {
            return Query()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ResourceName)
                .ToList();
        }

        public IList<Resource> SelectByLab(int? labId)
        {
            //procResourceSelect @Action='SelectByLabID'

            /*if @LabID is null
                Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID in (1, 9)	
			    order by ResourceName
		    else
			    Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID = case when @LabID = 0 then LabID else @LabID end
                order by ResourceName*/

            int[] defaultLabs = { 1, 9 };

            if (ConfigurationManager.AppSettings["DefaultLabs"] != null)
                defaultLabs = ConfigurationManager.AppSettings["DefaultLabs"].Split(',').Select(int.Parse).ToArray();

            IQueryable<Resource> query;

            if (labId == null)
                query = Query().Where(x => x.IsActive && defaultLabs.Contains(x.ProcessTech.Lab.LabID));
            else
            {
                if (labId.Value == 0)
                    query = Query().Where(x => x.IsActive);
                else
                    query = Query().Where(x => x.IsActive && x.ProcessTech.Lab.LabID == labId.Value);
            }

            IList<Resource> result = query.OrderBy(x => x.ProcessTech.Lab.Building.BuildingName).ThenBy(x => x.ProcessTech.Lab.LabName).ThenBy(x => x.ProcessTech.ProcessTechName).ThenBy(x => x.ResourceName).ToList();

            return result;
        }
    }
}
