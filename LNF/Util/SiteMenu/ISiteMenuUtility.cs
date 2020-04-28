namespace LNF.Util.SiteMenu
{
    public interface ISiteMenuUtility
    {
        string GetSiteMenu(int clientId, string target = null);
        string GetSiteMenu(string username, string target = null);
    }
}
