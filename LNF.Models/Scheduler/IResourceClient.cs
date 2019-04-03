using System;
using LNF.Models.Data;

namespace LNF.Models.Scheduler
{
    public interface IResourceClient : IPrivileged, IAuthorized
    {
        int ResourceClientID { get; set; }
        int ResourceID { get; set; }
        DateTime? Expiration { get; set; }
        int? EmailNotify { get; set; }
        int? PracticeResEmailNotify { get; set; }
        string ResourceName { get; set; }
        int AuthDuration { get; set; }
        bool ResourceIsActive { get; set; }
        string DisplayName { get; set; }
        string Email { get; set; }
        bool ClientActive { get; set; }
    }
}