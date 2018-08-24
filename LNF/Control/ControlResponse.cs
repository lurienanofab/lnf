using System;

namespace LNF.Control
{
    public abstract class ControlResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
    }
}
