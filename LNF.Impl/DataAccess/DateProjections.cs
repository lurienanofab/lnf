using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;

namespace LNF.Impl.DataAccess
{
    public enum DatePart
    {
        Year,
        Quarter,
        Month,
        DayOfYear,
        Day,
        Week,
        Hour,
        Minute,
        Second,
        Millisecond,
        Microsecond,
        Nanosecond
    }

    public static class DateProjections
    {
        private const string DateDiffFormat = "datediff({0}, ?1, ?2)";

        private static Dictionary<string, ISQLFunction> DateDiffFunctionCache = new Dictionary<string, ISQLFunction>();

        public static IProjection GetDate()
        {
            IProjection result = Projections.SqlFunction(GetGetDateFunction(), NHibernateUtil.DateTime);
            return result;
        }

        public static IProjection DateDiff(DatePart datepart, IProjection startDateProjection, IProjection endDateProjection)
        {
            string part = Enum.GetName(typeof(DatePart), datepart).ToLower();
            ISQLFunction sqlFunction = GetDateDiffFunction(part);

            var result = Projections.SqlFunction(sqlFunction, NHibernateUtil.Int32, startDateProjection, endDateProjection);

            return result;
        }

        private static ISQLFunction GetDateDiffFunction(string datepart)
        {
            ISQLFunction sqlFunction;

            if (!DateDiffFunctionCache.TryGetValue(datepart, out sqlFunction))
            {
                string functionTemplate = string.Format(DateDiffFormat, datepart);
                sqlFunction = new SQLFunctionTemplate(NHibernateUtil.Int32, functionTemplate);
                DateDiffFunctionCache[datepart] = sqlFunction;
            }

            return sqlFunction;
        }

        private static ISQLFunction GetGetDateFunction()
        {
            ISQLFunction sqlFunction;

            if (!DateDiffFunctionCache.TryGetValue("getdate", out sqlFunction))
            {
                string functionTemplate = "getdate()";
                sqlFunction = new SQLFunctionTemplate(NHibernateUtil.DateTime, functionTemplate);
                DateDiffFunctionCache["getdate"] = sqlFunction;
            }

            return sqlFunction;
        }
    }
}
