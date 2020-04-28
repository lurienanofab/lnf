using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Util.AutoEnd;
using NHibernate;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Util
{
    public class AutoEndUtility : IAutoEndUtility
    {
        private readonly ISessionManager _mgr;
        protected ISession Session => _mgr.Session;

        public AutoEndUtility(ISessionManager mgr)
        {
            _mgr = mgr;
        }

        public int FixAllAutoEndProblems(DateTime period)
        {
            return Session
                .CreateSQLQuery("EXEC dbo.BillingChecks_FixAutoEndProblems @Period = :Period")
                .SetParameter("Period", period)
                .UniqueResult<int>();
        }

        public int FixAutoEndProblem(DateTime period, int reservationId)
        {
            return Session
                .CreateSQLQuery("EXEC dbo.BillingChecks_FixAutoEndProblems @Period = :Period, @ReservationID = :ReservationID")
                .SetParameter("Period", period)
                .SetParameter("ReservationID", reservationId)
                .UniqueResult<int>();
        }

        public IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period)
        {
            var table = Session
                .CreateSQLQuery("EXEC dbo.BillingChecks_FindAutoEndProblems @Period = :Period")
                .SetParameter("Period", period)
                .FillTable();

            var result = new List<AutoEndProblem>();

            foreach (var row in table)
            {
                var item = new AutoEndProblem
                {
                    ReservationID = (int)row["ReservationID"],
                    AutoEndType = (string)row["AutoEndType"],
                    AutoEndReservation = (bool)row["AutoEndReservation"],
                    AutoEndResource = TimeSpan.FromMinutes((int)row["AutoEndResource"]),
                    Duration = TimeSpan.FromMinutes((double)row["Duration"]),
                    EndDateTime = (DateTime)row["EndDateTime"],
                    ActualEndDateTime = (DateTime?)row["ActualEndDateTime"],
                    ActualEndDateTimeExpected = (DateTime)row["ActualEndDateTimeExpected"],
                    Diff = TimeSpan.FromSeconds((int)row["DiffSeconds"]),
                    ActualEndDateTimeCorrected = (DateTime)row["ActualEndDateTimeCorrected"],
                    ChargeMultiplier = (double)row["ChargeMultiplier"]
                };

                result.Add(item);
            }

            return result;
        }
    }
}
