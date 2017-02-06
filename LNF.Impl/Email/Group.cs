using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Impl.Email
{
    public class Group : IEmailGroup
    {
        public Group(object entry)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<IEmailGroupMember> GetMembers()
        {
            throw new NotImplementedException();
        }

        public string GroupID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string GroupName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Empty
        {
            get { throw new NotImplementedException(); }
        }
    }
}
