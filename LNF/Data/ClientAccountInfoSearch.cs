using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public class ClientAccountInfoSearch
    {
        private IList<ClientAccountInfo> list;

        public ClientAccountInfoSearch()
        {
            list = DA.Current.Query<ClientAccountInfo>().ToList();
        }

        public ClientAccountInfoSearch(IList<ClientAccountInfo> items)
        {
            list = items;
        }

        public IList<ClientAccountInfo> Search(Func<ClientAccountInfo, bool> filter)
        {
            return list.Where(filter).ToList();
        }

        public ClientAccountInfo Find(Func<ClientAccountInfo, bool> filter)
        {
            return list.FirstOrDefault(filter);
        }
    }
}
