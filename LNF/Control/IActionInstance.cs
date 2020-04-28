namespace LNF.Control
{
    public interface IActionInstance : IControlInstance
    {
        string ActionName { get; set; }
    }
}