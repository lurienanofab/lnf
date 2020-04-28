using LNF.Inventory;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Inventory
{
    public class CategoryRepository : ApiClient, ICategoryRepository
    {
        public IEnumerable<ICategory> GetCategories()
        {
            throw new NotImplementedException();
        }

        public ICategory GetCategory(int catId)
        {
            throw new NotImplementedException();
        }
    }
}
