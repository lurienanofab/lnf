using LNF.Models.Data;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A menu item in the main system menu
    /// </summary>
    public class Menu : IDataItem
    {
        /// <summary>
        /// The unique id of a Menu
        /// </summary>
        public virtual int MenuID { get; set; }

        /// <summary>
        /// The unique id of a Menu parent
        /// </summary>
        public virtual int MenuParentID { get; set; }

        /// <summary>
        /// The text of a Menu
        /// </summary>
        public virtual string MenuText { get; set; }

        /// <summary>
        /// The url of a Menu
        /// </summary>
        public virtual string MenuURL { get; set; }

        /// <summary>
        /// The CSS class name
        /// </summary>
        public virtual string MenuCssClass { get; set; }

        /// <summary>
        /// The privilege required to make the Menu available to the current user (0 for always available)
        /// </summary>
        public virtual int MenuPriv { get; set; }

        /// <summary>
        /// Indicates the url should be opened in a new window
        /// </summary>
        public virtual bool NewWindow { get; set; }

        /// <summary>
        /// Indicates the url should open in the top frame
        /// </summary>
        public virtual bool TopWindow { get; set; }

        /// <summary>
        /// Indicates a Menu is a log out button
        /// </summary>
        public virtual bool IsLogout { get; set; }

        /// <summary>
        /// The order a Menu should appear under its parent
        /// </summary>
        public virtual int SortOrder { get; set; }

        /// <summary>
        /// Indicates a Menu is active (displayable)
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Indicates a Menu is deleted (never displayable)
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets a value that indicates if the Menu should be displayed based on a specified ClientPrivilege
        /// </summary>
        /// <param name="c">An object that implements the IPrivileged interface</param>
        /// <returns>True if the menu should be displayed, otherwise false</returns>
        public virtual bool IsVisible(IPrivileged c)
        {
            bool result = true;

            if (MenuPriv > 0)
                result = ((int)c.Privs & MenuPriv) > 0;
            else if (MenuPriv < 0)
                result = !(((int)c.Privs & (-1 * MenuPriv)) > 0);

            return result;
        }
    }
}
