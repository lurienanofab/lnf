using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Models.Control
{
    public class BlockItem
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string IPAddress { get; set; }
        public string Description { get; set; }
        public string MACAddress { get; set; }
    }
}
