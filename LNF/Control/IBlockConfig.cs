namespace LNF.Control
{
    public interface IBlockConfig
    {
        int ConfigID { get; set; }
        int BlockID { get; set; }
        int ModTypeID { get; set; }
        byte ModPosition { get; set; }
    }
}
