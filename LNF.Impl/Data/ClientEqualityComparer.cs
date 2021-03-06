﻿using LNF.Impl.Repository.Data;
using System.Collections.Generic;

namespace LNF.Impl.Data
{
    public class ClientEqualityComparer : IEqualityComparer<Client>
    {
        public bool Equals(Client x, Client y)
        {
            return x.ClientID == y.ClientID;
        }

        public int GetHashCode(Client obj)
        {
            return obj.ClientID.GetHashCode();
        }
    }
}
