using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Repository.Data
{
    public class Department : IDataItem
    {
        public Department() { }
        public virtual int DepartmentID { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual Org Org { get; set; }
    }
}
