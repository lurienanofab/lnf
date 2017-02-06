using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public class DataFeedModel<T>
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public string Name { get; set; }
        public bool Private { get; set; }
        public bool Active { get; set; }
        public Dictionary<string, IEnumerable<T>> Data { get; set; }
    }
}