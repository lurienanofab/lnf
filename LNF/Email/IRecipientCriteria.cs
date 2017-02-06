using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email
{
    public interface IRecipientCriteria
    {
        IEnumerable<MassEmailRecipient> GetRecipients();
    }
}
