using System.Linq;

namespace LNF.Impl.Repository.Feedback
{
    /// <summary>
    /// All available issues status values
    /// </summary>
    public static class IssueStatus
    {
        /// <summary>
        /// Submitted status
        /// </summary>
        public static readonly string Submitted = "Submitted";

        /// <summary>
        /// Confirmed status
        /// </summary>
        public static readonly string Confirmed = "Confirmed";

        /// <summary>
        /// Disputed status
        /// </summary>
        public static readonly string Disputed = "Disputed";

        /// <summary>
        /// Removed status
        /// </summary>
        public static readonly string Removed = "Removed";

        /// <summary>
        /// Gets a list of all IssueStatus values
        /// </summary>
        /// <param name="includeAll">Indicates if an 'All' option should be included at the beginning of the list</param>
        /// <returns>An array of status strings</returns>
        public static string[] GetList(bool includeAll)
        {
            var result = typeof(IssueStatus)
                .GetFields()
                .Select(x => x.GetValue(null).ToString())
                .ToList();

            if (includeAll)
                result.Insert(0, "All");

            return result.ToArray();
        }
    }
}
