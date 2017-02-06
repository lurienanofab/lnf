using System;
using System.Threading.Tasks;

namespace LNF.Service
{
    public class ServiceJob
    {
        public Guid ID { get; }
        public string Name { get; set; }
        public Func<Task<bool>> Action { get; set; }

        public ServiceJob(Guid id)
        {
            ID = id;
        }
    }
}
