using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.DataTables
{
    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Orderable { get; set; }
        public ColumnSearch Search { get; set; }
        public bool Searchable { get; set; }
    }
}
