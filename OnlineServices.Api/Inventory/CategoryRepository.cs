using LNF.Inventory;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Inventory
{
    public class CategoryRepository : ApiClient, ICategoryRepository
    {
        internal CategoryRepository(IRestClient rc) : base(rc) { }

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
