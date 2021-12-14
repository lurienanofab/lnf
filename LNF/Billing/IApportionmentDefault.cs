namespace LNF.Billing
{
    public interface IApportionmentDefault
    {
        int RoomID { get; set; }
        int AccountID { get; set; }
        double Percentage { get; set; }
    }
}
