using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Scheduler;
using NHibernate.Transform;
using System.Collections.Generic;

namespace LNF.Impl.Scheduler
{
    public class KioskRepository : RepositoryBase, IKioskRepository
    {
        public KioskRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IKiosk> GetKiosks()
        {
            return Session.CreateSQLQuery("SELECT k.KioskID, k.KioskName, k.KioskIP, ISNULL(klab.LabID, 0) AS LabID FROM sselScheduler.dbo.Kiosk k LEFT JOIN sselScheduler.dbo.KioskLab klab ON klab.KioskID = k.KioskID")
                    .SetResultTransformer(Transformers.AliasToBean<KioskItem>())
                    .List<KioskItem>();
        }
    }
}
