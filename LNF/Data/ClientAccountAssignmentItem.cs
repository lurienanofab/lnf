namespace LNF.Data
{
    public class ClientAccountAssignmentItem : IClientAccountAssignment
    {
        public int ClientManagerID { get; set; }
        public int ClientOrgID { get; set; }
        public int ManagerOrgID { get; set; }
        public bool ClientManagerActive { get; set; }
        public int ManagerClientAccountID { get; set; }
        public bool ManagerClientAccountActive { get; set; }
        public bool Manager { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public bool AccountActive { get; set; }
        public string ManagerEmail { get; set; }
        public bool ManagerClientOrgActive { get; set; }
        public string ManagerLastName { get; set; }
        public string ManagerFirstName { get; set; }
        public ClientPrivilege ManagerPrivs { get; set; }
        public bool ManagerClientActive { get; set; }
        public int EmployeeClientAccountID { get; set; }
        public bool EmployeeClientAccountActive { get; set; }
        public string EmployeeEmail { get; set; }
        public bool EmployeeClientOrgActive { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFirstName { get; set; }
        public ClientPrivilege EmployeePrivs { get; set; }
        public bool EmployeeClientActive { get; set; }
        public bool HasDryBox { get; set; }
    }
}
