using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Control
{
    public interface IControlInstance
    {
        int Index { get; set; }
        int Point { get; set; }
        int ActionID { get; set; }
        string Name { get; set; }
    }
}
