using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Control;

namespace LNF.Control
{
    public class BlockResponse : ControlResponse
    {
        public BlockState BlockState { get; set; }
    }
}
