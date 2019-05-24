using LNF.Models.Control;

namespace OnlineServices.Api.Control
{
    public class ControlService : IControlService
    {
        public IInterlockManager Interlock { get; }

        public ControlService(IInterlockManager interlock)
        {
            Interlock = interlock;
        }
    }
}
