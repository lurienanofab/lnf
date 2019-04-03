namespace LNF.Models.Data
{
    public class DryBoxItem : IDryBox
    {
        public int DryBoxID { get; set; }
        public string DryBoxName { get; set; }
        public bool Visible { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
