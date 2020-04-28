namespace LNF.Scheduler
{
    public interface IClientSetting
    {
        int ClientID { get; set; }
        int? BuildingID { get; set; }
        int? LabID { get; set; }
        int? DefaultView { get; set; }
        int? BeginHour { get; set; }
        int? EndHour { get; set; }
        string WorkDays { get; set; }
        bool? EmailCreateReserv { get; set; }
        bool? EmailModifyReserv { get; set; }
        bool? EmailDeleteReserv { get; set; }
        bool? EmailInvited { get; set; }
        string AccountOrder { get; set; }

        int DefaultBuildingID { get; }
        int DefaultLabID { get; }
        int DefaultDefaultView { get; }
        int DefaultBeginHour { get; }
        int DefaultEndHour { get; }
        string DefaultWorkDays { get; }
        bool DefaultEmailCreateReserv { get; }
        bool DefaultEmailModifyReserv { get; }
        bool DefaultEmailDeleteReserv { get; }
        bool DefaultEmailInvited { get; }
        string DefaultAccountOrder { get; }

        int GetBuildingID();
        int GetLabID();
        ViewType GetDefaultView();
        int GetBeginHour();
        int GetEndHour();
        string GetWorkDays();
        bool GetEmailCreateReserv();
        bool GetEmailModifyReserv();
        bool GetEmailDeleteReserv();
        bool GetEmailInvited();
        string GetAccountOrder();
        bool IsValid();
    }
}
