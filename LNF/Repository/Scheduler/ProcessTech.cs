using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class ProcessTech : IDataItem
    {
        public virtual int ProcessTechID { get; set; }
        public virtual ProcessTechGroup Group { get; set; }
        public virtual Lab Lab { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IQueryable<Resource> GetResources()
        {
            return DA.Current.Query<Resource>().Where(x => x.ProcessTech.ProcessTechID == ProcessTechID);
        }
    }
}
