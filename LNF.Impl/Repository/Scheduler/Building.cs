using LNF.DataAccess;
using LNF.Scheduler;
using System.Linq;

namespace LNF.Impl.Repository.Scheduler
{
    public class Building : IBuilding, IDataItem
    {
        public virtual int BuildingID { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual string BuildingDescription { get; set; }
        public virtual bool BuildingIsActive { get; set; }

        public virtual IQueryable<Lab> Labs(ISession session)
        {
            return session.Query<Lab>().Where(x => x.Building.BuildingID == BuildingID);
        }
    }
}
