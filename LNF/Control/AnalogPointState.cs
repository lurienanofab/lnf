using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Control;

namespace LNF.Control
{
    public class AnalogPointState : ControlResponse
    {
        public Point Point { get; set; }
        public int State { get; set; }
    }
}
