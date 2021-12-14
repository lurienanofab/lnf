namespace LNF.Scheduler
{
    public interface IKioskConfig
    {
        IKioskRedirects Redirects { get; set; }
        string GetRedirectUrl(string ipaddr);
    }
}
