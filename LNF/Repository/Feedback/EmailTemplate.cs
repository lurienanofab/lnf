using System;

namespace LNF.Repository.Feedback
{
    /// <summary>
    /// Defines templates used for sending emails to staff and users from the Feedback system
    /// </summary>
    public class EmailTemplate : IDataItem
    {
        /// <summary>
        /// The unique id of the template
        /// </summary>
        public virtual int EmailTemplateID { get; set; }

        /// <summary>
        /// The template name
        /// </summary>
        public virtual string TemplateName { get; set; }

        /// <summary>
        /// The template value
        /// </summary>
        public virtual string TemplateValue { get; set; }

        /// <summary>
        /// The date when the template was last modified
        /// </summary>
        public virtual DateTime LastModified { get; set; }

        /// <summary>
        /// The unique id of the user that modified the template
        /// </summary>
        public virtual int ClientID { get; set; }
    }
}
