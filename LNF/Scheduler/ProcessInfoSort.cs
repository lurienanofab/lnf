using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ProcessInfoSort : IEnumerable<IProcessInfo>
    {
        private readonly List<ProcessInfoSortItem> _items = new List<ProcessInfoSortItem>();

        public ProcessInfoSort(IEnumerable<IProcessInfo> items)
        {
            Fill(items);
        }

        private void Fill(IEnumerable<IProcessInfo> items)
        {
            _items.Clear();
            var ordered = items.OrderBy(x => x.Order).ToList();
            foreach (var pinfo in ordered)
            {
                _items.Add(new ProcessInfoSortItem(pinfo));
            }
        }

        public void MoveDown(int processInfoId)
        {
            var current = _items.First(x => x.Item.ProcessInfoID == processInfoId);

            current.Sort += 15;

            var list = _items.OrderBy(x => x.Sort).Select(x => x.Item).ToList();

            for (var i = 0; i < list.Count; i++)
            {
                list[i].Order = i;
            }

            Fill(list);
        }

        public void MoveUp(int processInfoId)
        {
            var current = _items.First(x => x.Item.ProcessInfoID == processInfoId);

            current.Sort -= 15;

            var list = _items.OrderBy(x => x.Sort).Select(x => x.Item).ToList();

            for (var i = 0; i < list.Count; i++)
            {
                list[i].Order = i;
            }

            Fill(list);
        }

        public IEnumerator<IProcessInfo> GetEnumerator()
        {
            return _items.Select(x => x.Item).OrderBy(x => x.Order).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ProcessInfoSortItem
    {
        public ProcessInfoSortItem(IProcessInfo pinfo)
        {
            Sort = pinfo.Order * 10;
            Item = pinfo;
        }

        public int Sort { get; set; }
        public IProcessInfo Item { get; }
    }
}
