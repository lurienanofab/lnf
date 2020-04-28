using LNF.Control;
using LNF.DataAccess;
using System.Collections.Generic;

namespace LNF.Impl.Repository.Control
{
    public class Block : IBlock, IDataItem
    {
        private IList<Point> _Points;
        private IList<BlockConfig> _Configurations;

        public virtual int BlockID { get; set; }
        public virtual string BlockName { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual string Description { get; set; }
        public virtual string MACAddress { get; set; }

        public virtual IList<Point> Points
        {
            get { return _Points; }
        }

        public virtual IList<BlockConfig> Configurations
        {
            get { return _Configurations; }
        }

        public Block()
        {
            _Points = new List<Point>();
            _Configurations = new List<BlockConfig>();
        }
    }
}
