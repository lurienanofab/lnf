using System;

namespace LNF.Reporting
{
    public interface IClientEmailPreference
    {
        int ClientEmailPreferenceID { get; set; }
        int EmailPreferenceID { get; set; }
        int ClientID { get; set; }
        DateTime EnableDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}
