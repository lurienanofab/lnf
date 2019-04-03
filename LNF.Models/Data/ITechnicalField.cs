namespace LNF.Models.Data
{
    public interface ITechnicalField
    {
        /// <summary>
        /// The unique id of a technical field
        /// </summary>
        int TechnicalFieldID { get; set; }

        /// <summary>
        /// The name of a technical field
        /// </summary>
        string TechnicalFieldName { get; set; }
    }
}
