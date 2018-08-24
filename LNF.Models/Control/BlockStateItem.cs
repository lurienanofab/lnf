using System;
using System.Linq;

namespace LNF.Models.Control
{
    public class BlockStateItem
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string IPAddress { get; set; }
        public PointStateItem[] Points { get; set; }

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
