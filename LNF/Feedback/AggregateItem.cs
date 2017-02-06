namespace LNF.Feedback
{
    public class AggregateItem
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public int TotalNegative { get; set; }
        public int TotalPositive { get; set; }
        public int Staff { get; set; }
        public int Internal { get; set; }
        public int External { get; set; }
        public int Safety { get; set; }
        public int Protocol { get; set; }
        public int Etiquette { get; set; }
    }
}
