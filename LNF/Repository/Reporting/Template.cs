namespace LNF.Repository.Reporting
{
    public class Template : IDataItem
    {
        public virtual int TemplateID { get; set; }
        public virtual string TemplateName { get; set; }
        public virtual string TemplateContent { get; set; }
        public virtual string Report { get; set; }
    }
}
