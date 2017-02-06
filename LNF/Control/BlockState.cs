using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Control;

namespace LNF.Control
{
    public class BlockState
    {
        public int BlockID {get; set; }
        public string BlockName { get; set; }
        public string IPAddress { get; set; }
        public PointState[] Points { get; set; }

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
