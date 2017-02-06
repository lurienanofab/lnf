using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Repository.Data
{
    public class SpecialTopic : IDataItem
    {
        public SpecialTopic() { }
        public virtual int SpecialTopicID { get; set; }
        public virtual string SpecialTopicName { get; set; }
    }
}
