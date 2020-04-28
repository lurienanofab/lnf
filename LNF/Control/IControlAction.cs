namespace LNF.Control
{
    public interface IControlAction
    {
        int ActionID { get; set; }
        string ActionName { get; set; }
        string ActionTableName { get; set; }
    }
}
