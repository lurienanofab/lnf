using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LNF.Impl.Util.Serialization
{
    public class DynamicContractResolver<T> : DefaultContractResolver where T : new()
    {
        private List<string> _propertiesToSerialize = null;

        public DynamicContractResolver()
        {
            _propertiesToSerialize = new List<string>();
        }

        public DynamicContractResolver(IEnumerable<string> list)
        {
            _propertiesToSerialize = list.ToList();
        }

        public DynamicContractResolver(Func<T, IEnumerable<string>> fn)
        {
            _propertiesToSerialize = fn.Invoke(new T()).ToList();
        }

        public DynamicContractResolver<T> AddProperty<TKey>(Expression<Func<T, TKey>> exp)
        {
            _propertiesToSerialize.Add(CommonTools.Utility.PropertyName(exp));
            return this;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(p => _propertiesToSerialize.Contains(p.PropertyName)).ToList();
        }
    }
}
