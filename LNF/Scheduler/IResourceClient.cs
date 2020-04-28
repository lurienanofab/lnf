using System;

namespace LNF.Scheduler
{
    public interface IResourceClient : IAuthorized
    {
        int ResourceClientID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        int AuthDuration { get; set; }
        bool ResourceIsActive { get; set; }
        string DisplayName { get; set; }
        string Email { get; set; }
        bool ClientActive { get; set; }
        DateTime? Expiration { get; set; }
        int? EmailNotify { get; set; }
        int? PracticeResEmailNotify { get; set; }
        DateTime? WarningDate(double authExpWarning);
    }
}