using System.Collections.Generic;

namespace LNF.Data
{
    public class NewsListItem
    {
        public int NewsID { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTicker { get; set; }
    }
}
