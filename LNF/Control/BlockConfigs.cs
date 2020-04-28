namespace LNF.Control
{
    public static class BlockConfigs
    {
        public static IBlockConfig Find(int blockId, byte modPosition)
        {
            return ServiceProvider.Current.Control.GetBlockConfig(blockId, modPosition);
        }
    }
}
