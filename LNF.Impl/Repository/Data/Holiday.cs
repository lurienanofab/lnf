using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class Holiday : IDataItem
    {
        public virtual int HolidayID { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime HolidayDate { get; set; }
    }
}
