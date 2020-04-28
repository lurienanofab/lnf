using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF
{
    public class FileCollection : IEnumerable<IFile>
    {
        private IList<IFile> _items = new List<IFile>();

        public FileCollection(string name, IEnumerable<IFile> items)
        {
            Name = name;
            _items = items.ToList();
        }

        public string Name { get; set; }

        public IFile this[int index]
        {
            get => _items[index];
        }

        public IFile this[string name]
        {
            get => _items.FirstOrDefault(x => x.FileName == name);
        }

        public int Count => _items.Count;

        public void Add(IFile item)
        {
            _items.Add(item);
        }

        public IEnumerator<IFile> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
