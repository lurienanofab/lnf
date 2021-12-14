using LNF.Data;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class DryBoxAssignmentLog : IDataItem
    {
        public virtual int DryBoxAssignmentLogID { get; set; }
        public virtual DryBoxAssignment DryBoxAssignment { get; set; }
        public virtual ClientAccount ClientAccount { get; set; }
        public virtual DateTime EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
        public virtual Client ModifiedBy { get; set; }
    }
}
