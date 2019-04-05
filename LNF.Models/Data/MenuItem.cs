namespace LNF.Models.Data
{
    public class MenuItem : IMenu
    {
        public int MenuID { get; set; }
        public int MenuParentID { get; set; }
        public string MenuText { get; set; }
        public string MenuURL { get; set; }
        public string MenuCssClass { get; set; }
        public int MenuPriv { get; set; }
        public bool NewWindow { get; set; }
        public bool TopWindow { get; set; }
        public bool IsLogout { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool IsVisible(IPrivileged c) => IsVisible(c, MenuPriv);

        public static bool IsVisible(IPrivileged c, int menuPriv)
        {
            bool result = true;

            if (menuPriv > 0)
                result = ((int)c.Privs & menuPriv) > 0;
            else if (menuPriv < 0)
                result = !(((int)c.Privs & (-1 * menuPriv)) > 0);

            return result;
        }
    }
}
