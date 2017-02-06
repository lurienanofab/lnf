using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Control;

namespace LNF.Control
{
    public static class BlockConfigUtility
    {
        public static BlockConfig Find(int blockId, int modPosition)
        {
            return DA.Current
                .Query<BlockConfig>()
                .FirstOrDefault(x => x.Block.BlockID == blockId && x.ModPosition == modPosition);
        }
    }
}
