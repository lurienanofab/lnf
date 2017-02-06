using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class Building : IDataItem
    {
        public virtual int BuildingID { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IQueryable<Lab> Labs()
        {
            return DA.Current.Query<Lab>().Where(x => x.Building.BuildingID == BuildingID);
        }
    }
}
