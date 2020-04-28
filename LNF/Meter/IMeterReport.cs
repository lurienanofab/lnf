namespace LNF.Meter
{
    public interface IMeterReport
    {
        int ReportID { get; set; }
        string ReportType { get; set; }
        string ReportName { get; set; }
        string Header { get; set; }
        double UnitCost { get; set; }
        string BorderColor { get; set; }
        string BackgroundColor { get; set; }
        string PointBorderColor { get; set; }
        string PointBackgroundColor { get; set; }
        bool Active { get; set; }
    }
}
