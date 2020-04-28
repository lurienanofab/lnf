namespace LNF.Feedback
{
    public interface INegativeIssue : IFeedbackIssue
    {
        int ResourceID { get; set; }
        bool Rule1 { get; set; }
        bool Rule2 { get; set; }
        bool Rule3 { get; set; }
        bool Rule4 { get; set; }
        bool Rule5 { get; set; }
        bool Rule6 { get; set; }
        bool Rule7 { get; set; }
        bool Rule8 { get; set; }
        bool Rule9 { get; set; }
        bool Rule10 { get; set; }
        bool Rule11 { get; set; }
        bool Rule12 { get; set; }
        bool Rule13 { get; set; }
    }
}
