using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LNF.DataTables
{
    [DataContract]
    public class Response
    {
        public Response() { }

        [DataMember(Name = "draw")]
        public int Draw { get; set; }
        [DataMember(Name = "recordsTotal")]
        public int RecordsTotal { get; set; }
        [DataMember(Name = "recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [DataMember(Name = "data")]
        public IList<IDictionary<string, object>> Data { get; set; }
    }
}
