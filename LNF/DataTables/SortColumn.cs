using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.DataTables
{
    public enum Direction
    {
        Ascending = 1,
        Descending = 2
    }

    public class SortColumn
    {
        public int Index { get; set; }
        public Direction Direction { get; set; }
    }
}
