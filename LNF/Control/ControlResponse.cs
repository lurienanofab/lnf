using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Control
{
    public abstract class ControlResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
    }
}
