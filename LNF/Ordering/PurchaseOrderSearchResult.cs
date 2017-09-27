using LNF.Models.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Ordering
{
    public class PurchaseOrderSearchResult
    {
        private static readonly MemoryCache _cache;
        private static readonly CacheItemPolicy _policy;

        static PurchaseOrderSearchResult()
        {
            _cache = new MemoryCache("PurchaseOrderSearchResult");
            _policy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15) };
        }

        public int ClientID { get; set; }

        public DateTime CreatedAt { get; set; }

        public IList<PurchaseOrderSearchModel> Items { get; set; }

        private static IList<PurchaseOrderSearchResult> GetCollection()
        {
            IList<PurchaseOrderSearchResult> result;

            if (_cache["items"] == null)
            {
                result = new List<PurchaseOrderSearchResult>();
                _cache.Add("items", result, _policy);
            }
            else
            {
                result = (IList<PurchaseOrderSearchResult>)_cache["items"];
            }

            return result;
        }

        public static PurchaseOrderSearchResult Get(Func<PurchaseOrderSearchResult, bool> filter)
        {
            return GetCollection().FirstOrDefault(filter);
        }

        public static PurchaseOrderSearchResult Set(int clientId, IEnumerable<PurchaseOrderSearchModel> items)
        {
            PurchaseOrderSearchResult result = new PurchaseOrderSearchResult()
            {
                ClientID = clientId,
                CreatedAt = DateTime.Now,
                Items = items.ToList()
            };

            var col = GetCollection();
            var item = col.FirstOrDefault(x => x.ClientID == clientId);

            bool update = false;

            if (item == null)
            {
                col.Add(result);
                update = true;
            }
            else
            {
                var index = col.IndexOf(item);
                if (index != -1)
                {
                    col[index] = result;
                    update = true;
                }
            }

            if (update)
                _cache.Set("items", col, _policy);

            return result;
        }

        public static void Delete(int clientId)
        {
            var col = GetCollection();
            var item = col.FirstOrDefault(x => x.ClientID == clientId);
            if (item != null)
            {
                col.Remove(item);
                _cache.Set("items", col, _policy);
            }
        }
    }
}
