using LNF.Scheduler;
using System;
using System.Threading.Tasks;

namespace LNF.Scripting.Entities.Scheduler
{
    public class Resource
    {
        private IResource item;

        internal Resource(IResource i)
        {
            item = i;
        }

        public int ResourceID { get { return item.ResourceID; } }

        public string ResourceName { get { return item.ResourceName; } }

        public async Task<string> GetInterlockStatus()
        {
            await Task.FromResult(string.Empty);
            throw new NotImplementedException();
            //return await item.GetInterlockStatus();
        }
    }
}
