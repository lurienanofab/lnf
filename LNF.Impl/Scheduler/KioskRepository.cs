using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Scheduler;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Caching;
using Newtonsoft.Json;

namespace LNF.Impl.Scheduler
{
    public class KioskRepository : RepositoryBase, IKioskRepository
    {
        public KioskRepository(ISessionManager mgr) : base(mgr) { }

        public IKioskConfig GetKioskConfig()
        {
            var filePath = Path.Combine(ConfigurationManager.AppSettings["SecurePath"], "kiosks", "kiosks.json");
            var json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<KioskConfig>(json);
            return result;
        }

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

        public bool RefreshCache()
        {
            if (Cache.Contains("Kiosks"))
            {
                Cache.Remove("Kiosks");
                return true;
            }

            return false;
        }
    }
}
