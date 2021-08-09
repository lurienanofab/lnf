using System;
using System.Collections.Generic;

namespace LNF.Help
{
    public class FeeSection
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<FeeLink> Links { get; set; }
    }
}
