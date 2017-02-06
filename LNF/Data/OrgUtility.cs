using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public static class OrgUtility
    {
        public static Org GetPrimaryOrg()
        {
            return DA.Current.Query<Org>().FirstOrDefault(x => x.PrimaryOrg);
        }

        public static IList<Org> SelectActive()
        {
            IList<Org> result = DA.Current.Query<Org>().Where(x => x.Active).OrderBy(x => x.OrgName).ToList();
            return result;
        }
    }
}
