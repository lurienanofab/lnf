using System;
using System.Linq;

namespace LNF.Models.Control
{
    public class BlockStateModel
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public string IPAddress { get; set; }
        public PointStateModel[] Points { get; set; }

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
