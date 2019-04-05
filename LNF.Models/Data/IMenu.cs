namespace LNF.Models.Data
{
    public interface IMenu
    {
        bool Active { get; set; }
        bool Deleted { get; set; }
        bool IsLogout { get; set; }
        string MenuCssClass { get; set; }
        int MenuID { get; set; }
        int MenuParentID { get; set; }
        int MenuPriv { get; set; }
        string MenuText { get; set; }
        string MenuURL { get; set; }
        bool NewWindow { get; set; }
        int SortOrder { get; set; }
        bool TopWindow { get; set; }
        bool IsVisible(IPrivileged c);
    }
}