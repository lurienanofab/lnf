using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Control
{
    public class BlockState
    {
        public int BlockID {get; set; }
        public string BlockName { get; set; }
        public string IPAddress { get; set; }
        public IEnumerable<PointState> Points { get; set; }

        public bool GetPointState(int pointId)
        {
            return Points.First(x => x.PointID == pointId).State;
        }

        public byte[] GetAnalogPointState(int pointId)
        {
            throw new NotImplementedException();
        }
    }
}
