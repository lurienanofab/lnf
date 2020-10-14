using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Scheduler;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace LNF.Impl.Scheduler
{
    public class KioskRepository : RepositoryBase, IKioskRepository
    {
        public KioskRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IKiosk> GetKiosks()
        {
            IEnumerable<IKiosk> kiosks;

            if (!Cache.Contains("Kiosks"))
            {
                kiosks = Session.CreateSQLQuery("SELECT k.KioskID, k.KioskName, k.KioskIP, ISNULL(klab.LabID, 0) AS LabID FROM sselScheduler.dbo.Kiosk k LEFT JOIN sselScheduler.dbo.KioskLab klab ON klab.KioskID = k.KioskID")
                    .SetResultTransformer(Transformers.AliasToBean<KioskItem>())
                    .List<KioskItem>();
                Cache.Add("Kiosks", kiosks, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) });
            }
            else
            {
                kiosks = (IEnumerable<IKiosk>)Cache["Kiosks"];
            }

            return kiosks;
        }
    }
}
