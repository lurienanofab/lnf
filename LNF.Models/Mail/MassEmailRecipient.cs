using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public class MassEmailRecipient
    {
        public int ClientID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsStaff { get; set; }
    }

    public class MassEmailRecipientComparer : IEqualityComparer<MassEmailRecipient>
    {
        public bool Equals(MassEmailRecipient x, MassEmailRecipient y)
        {
            return x.Email == y.Email;
        }

        public int GetHashCode(MassEmailRecipient obj)
        {
            return obj.Email.GetHashCode();
        }
    }
}
