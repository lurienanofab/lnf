using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Control;

namespace LNF.Control
{
    public class PointResponse : ControlResponse
    {
        public int PointID { get; set; }
        public int BlockID { get; set; }
        public string PointName { get; set; }
    }
}
