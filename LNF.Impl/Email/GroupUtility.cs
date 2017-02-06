using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Email
{
    public class GroupUtility : IEmailGroupUtility
    {
        private IEmailProvider _provider;

        public GroupUtility(IEmailProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<IEmailGroup> RetrieveAllGroups()
        {
            throw new NotImplementedException();
        }

        public IEmailGroup RetrieveGroup(string groupId)
        {
            throw new NotImplementedException();
        }

        public IEmailGroup RetrieveGroup(Resource resource)
        {
            throw new NotImplementedException();
        }

        public IEmailGroup CreateGroup(string groupId, string groupName, string description, int emailPermission)
        {
            throw new NotImplementedException();
        }
    }
}
