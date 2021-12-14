using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class HolidayRepository : ApiClient, IHolidayRepository
    {
        internal HolidayRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<IHoliday> GetHolidays(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IHoliday> GetHolidays(DateTime now)
        {
            throw new NotImplementedException();
        }

        public bool IsHoliday(DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}
