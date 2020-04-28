namespace LNF.Scheduler
{
    public class ReservationTime
    {
        public double Value { get; set; }
        public string Text { get; set; }

        public static ReservationTime Create(double value, string text)
        {
            return new ReservationTime() { Value = value, Text = text };
        }
    }
}