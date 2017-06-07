using System.Collections.Generic;

namespace LNF.Models.Reporting
{
    public class ClientItemEqualityComparer : IEqualityComparer<ClientItem>
    {
        public bool Equals(ClientItem x, ClientItem y)
        {
            return x.ClientID == y.ClientID && x.Email == y.Email;
        }

        public int GetHashCode(ClientItem obj)
        {
            return new { obj.ClientID, obj.Email }.GetHashCode();
        }
    }
}
