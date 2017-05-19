namespace LNF.Repository.Reporting
{
    public class EmailPreference : IDataItem
    {
        public virtual int EmailPreferenceID { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
    }
}
