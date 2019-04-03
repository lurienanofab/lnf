namespace LNF.Models.Data
{
    public interface IDryBox
    {
        int DryBoxID { get; set; }
        string DryBoxName { get; set; }
        bool Visible { get; set; }
        bool Active { get; set; }
        bool Deleted { get; set; }
    }
}