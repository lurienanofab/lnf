namespace LNF.Control
{
    public class AnalogPointState : ControlResponse
    {
        public IPoint Point { get; set; }
        public int State { get; set; }
    }
}
