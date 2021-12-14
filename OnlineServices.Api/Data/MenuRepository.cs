using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class MenuRepository : ApiClient, IMenuRepository
    {
        internal MenuRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<IMenu> GetMenuItems()
        {
            throw new NotImplementedException();
        }
    }
}
