namespace LNF.Control
{
    public interface IPoint
    {
        int Index { get; set; }
        int BlockID { get; set; }
        string BlockName { get; set; }
        string IPAddress { get; set; }
        int PointID { get; set; }
        int ModPosition { get; set; }
        int Offset { get; set; }
        string InstanceName { get; set; }
        int ActionID { get; set; }
        string ActionName { get; set; }
    }
}
