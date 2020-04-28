namespace LNF.Impl.Repository.Feedback
{
    /// <summary>
    /// A negative feedback issue
    /// </summary>
    public class NegativeIssue : FeedbackIssue
    {
        private static readonly string[] _rules =
        {
            "The highest priority is safety",
            "Do not work alone with chemicals",
            "Use proper PPE",
            "Wear safety glasses",
            "No unattended chemicals or processes",
            "Properly identify all chemicals or processes",
            "Return workspace to clean status after use",
            "Do not assume, do not guess",
            "Improper equipment operation",
            "Community needs take precedence over individual needs",
            "Respect the lab, its equipment and everyone else",
            "Be responsible for assigned task (e.g. Lab Clean)",
            "Escalate issues to lab manager if necessary"
        };

        /// <summary>
        /// The unique id of the resource related to this issue
        /// </summary>
        public virtual int ResourceID { get; set; }

        /// <summary>
        /// Indicates if Rule1 was violated
        /// </summary>

        public virtual bool Rule1 { get; set; }

        /// <summary>
        /// Indicates if Rule2 was violated
        /// </summary>
        public virtual bool Rule2 { get; set; }

        /// <summary>
        /// Indicates if Rule3 was violated
        /// </summary>
        public virtual bool Rule3 { get; set; }

        /// <summary>
        /// Indicates if Rule4 was violated
        /// </summary>
        public virtual bool Rule4 { get; set; }

        /// <summary>
        /// Indicates if Rule5 was violated
        /// </summary>
        public virtual bool Rule5 { get; set; }

        /// <summary>
        /// Indicates if Rule6 was violated
        /// </summary>
        public virtual bool Rule6 { get; set; }

        /// <summary>
        /// Indicates if Rule7 was violated
        /// </summary>
        public virtual bool Rule7 { get; set; }

        /// <summary>
        /// Indicates if Rule8 was violated
        /// </summary>
        public virtual bool Rule8 { get; set; }

        /// <summary>
        /// Indicates if Rule9 was violated
        /// </summary>
        public virtual bool Rule9 { get; set; }

        /// <summary>
        /// Indicates if Rule10 was violated
        /// </summary>
        public virtual bool Rule10 { get; set; }

        /// <summary>
        /// Indicates if Rule11 was violated
        /// </summary>
        public virtual bool Rule11 { get; set; }

        /// <summary>
        /// Indicates if Rule12 was violated
        /// </summary>
        public virtual bool Rule12 { get; set; }

        /// <summary>
        /// Indicates if Rule13 was violated
        /// </summary>
        public virtual bool Rule13 { get; set; }

        /// <summary>
        /// Gets the text for a rule
        /// </summary>
        /// <param name="index">The rule index (1 based) - for example for Rule1 index = 1</param>
        /// <returns>A rule text string value</returns>
        public static string GetRuleText(int index)
        {
            return _rules[index - 1];
        }
    }
}
