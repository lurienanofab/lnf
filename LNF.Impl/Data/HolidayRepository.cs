using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class HolidayRepository : RepositoryBase, IHolidayRepository
    {
        public HolidayRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IHoliday> GetHolidays(DateTime sd, DateTime ed)
        {
            return Session.Query<Holiday>().Where(x => x.HolidayDate >= sd && x.HolidayDate < ed).CreateModels<IHoliday>();
        }

        public IEnumerable<IHoliday> GetHolidays(DateTime now)
        {
            var holidays = Session
                .CreateSQLQuery("EXEC dbo.Holiday_Select @Action = 'Check', @sDate = :sdate")
                .SetParameter("sdate", now.AddDays(-14))
                .List<Holiday>();

            return holidays.CreateModels<IHoliday>();
        }

        public bool IsHoliday(DateTime now)
        {
            var dt = Session.Command()
                .Param("Action", "IsHoliday")
                .Param("sDate", now)
                .FillDataTable("sselData.dbo.Holiday_Select");

            var result = dt.Rows.Count > 0;

            return result;
        }
    }
}
