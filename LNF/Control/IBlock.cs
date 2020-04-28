namespace LNF.Control
{
    public interface IBlock
    {
        int BlockID { get; set; }
        string BlockName { get; set; }
        string IPAddress { get; set; }
        string Description { get; set; }
        string MACAddress { get; set; }
    }
}
