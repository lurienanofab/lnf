using LNF.Data;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class Holiday : IHoliday, IDataItem
    {
        public virtual int HolidayID { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime HolidayDate { get; set; }
    }
}
