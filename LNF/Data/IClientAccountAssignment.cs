namespace LNF.Data
{
    public interface IClientAccountAssignment
    {
        int ClientManagerID { get; set; }
        int ClientOrgID { get; set; }
        int ManagerOrgID { get; set; }
        bool ClientManagerActive { get; set; }
        int ManagerClientAccountID { get; set; }
        bool ManagerClientAccountActive { get; set; }
        bool Manager { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        string AccountNumber { get; set; }
        string ShortCode { get; set; }
        bool AccountActive { get; set; }
        string ManagerEmail { get; set; }
        bool ManagerClientOrgActive { get; set; }
        string ManagerLastName { get; set; }
        string ManagerFirstName { get; set; }
        ClientPrivilege ManagerPrivs { get; set; }
        bool ManagerClientActive { get; set; }
        int EmployeeClientAccountID { get; set; }
        bool EmployeeClientAccountActive { get; set; }
        string EmployeeEmail { get; set; }
        bool EmployeeClientOrgActive { get; set; }
        string EmployeeLastName { get; set; }
        string EmployeeFirstName { get; set; }
        ClientPrivilege EmployeePrivs { get; set; }
        bool EmployeeClientActive { get; set; }
        bool HasDryBox { get; set; }
    }
}
