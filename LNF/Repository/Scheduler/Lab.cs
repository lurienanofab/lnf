using LNF.Repository.Data;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class Lab : IDataItem
    {
        public virtual int LabID { get; set; }
        public virtual Building Building { get; set; }
        public virtual string LabName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }

        /// <summary>
        /// The room this lab belongs to - can be null
        /// </summary>
        public virtual Room Room { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual IQueryable<ProcessTech> GetProcessTechs()
        {
            return DA.Current.Query<ProcessTech>().Where(x => x.Lab.LabID == LabID);
        }
    }
}
