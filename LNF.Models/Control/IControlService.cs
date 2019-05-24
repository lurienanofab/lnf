namespace LNF.Models.Control
{
    public interface IControlService
    {
        IInterlockManager Interlock { get; }
    }
}
