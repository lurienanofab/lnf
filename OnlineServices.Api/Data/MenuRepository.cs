using LNF.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class MenuRepository : ApiClient, IMenuRepository
    {
        public IEnumerable<IMenu> GetMenuItems()
        {
            throw new NotImplementedException();
        }
    }
}
