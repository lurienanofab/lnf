using LNF.DataAccess;
using System.Linq;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessTech : IDataItem
    {
        public virtual int ProcessTechID { get; set; }
        public virtual ProcessTechGroup Group { get; set; }
        public virtual Lab Lab { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IQueryable<Resource> GetResources(NHibernate.ISession session)
        {
            return session.Query<Resource>().Where(x => x.ProcessTech.ProcessTechID == ProcessTechID);
        }
    }
}
