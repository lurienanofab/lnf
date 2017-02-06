using LNF.Models.Data;

namespace LNF.Repository.Data
{
    public class ClientAccountAssignment : IDataItem
    {
        public virtual int ClientManagerID { get; set; }
        public virtual int ClientOrgID { get; set; }
        public virtual int ManagerOrgID { get; set; }
        public virtual bool ClientManagerActive { get; set; }
        public virtual int ManagerClientAccountID { get; set; }
        public virtual bool ManagerClientAccountActive { get; set; }
        public virtual bool Manager { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual bool AccountActive { get; set; }
        public virtual string ManagerEmail { get; set; }
        public virtual bool ManagerClientOrgActive { get; set; }
        public virtual string ManagerLastName { get; set; }
        public virtual string ManagerFirstName { get; set; }
        public virtual ClientPrivilege ManagerPrivs { get; set; }
        public virtual bool ManagerClientActive { get; set; }
        public virtual int EmployeeClientAccountID { get; set; }
        public virtual bool EmployeeClientAccountActive { get; set; }
        public virtual string EmployeeEmail { get; set; }
        public virtual bool EmployeeClientOrgActive { get; set; }
        public virtual string EmployeeLastName { get; set; }
        public virtual string EmployeeFirstName { get; set; }
        public virtual ClientPrivilege EmployeePrivs { get; set; }
        public virtual bool EmployeeClientActive { get; set; }
        public virtual bool HasDryBox { get; set; }

        public virtual string GetAccountProject()
        {
            return Account.GetProject(AccountNumber);
        }

        public virtual bool IsAssigned()
        {
            return EmployeeClientAccountID > 0;
        }

        public virtual bool Display()
        {
            return ClientManagerActive
                && ManagerClientActive
                && ManagerClientOrgActive
                && ManagerClientAccountActive
                && EmployeeClientActive
                && EmployeeClientOrgActive
                && AccountActive
                && Manager;
        }

        public virtual string GetEmployeeDisplayName()
        {
            return string.Format("{0}, {1}", EmployeeLastName, EmployeeFirstName);
        }

        public virtual string GetManagerDisplayName()
        {
            return string.Format("{0}, {1}", ManagerLastName, ManagerFirstName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ClientAccountAssignment)) return false;
            ClientAccountAssignment item = obj as ClientAccountAssignment;
            return item.ClientManagerID == ClientManagerID
                && item.ManagerClientAccountID == ManagerClientAccountID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", ClientManagerID, ManagerClientAccountID).GetHashCode();
        }
    }
}
