using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    [Serializable]
    public class TreeViewItem : IEnumerable<TreeViewItem>
    {
        private List<TreeViewItem> children;

        public TreeViewItem(string text)
        {
            Text = text;
            children = new List<TreeViewItem>();
        }

        public TreeViewItem() : this(string.Empty) { }

        public string Text { get; set; }

        public TreeViewItem this[int index]
        {
            get
            {
                return children[index];
            }
        }

        public void Add(TreeViewItem child)
        {
            children.Add(child);
        }

        public void Add(string text)
        {
            Add(new TreeViewItem(text));
        }

        public int Count
        {
            get { return children.Count; }
        }

        public IEnumerator<TreeViewItem> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [ParseChildren(true)]
    [ToolboxData("<{0}:TreeView runat=server></{0}:TreeView>")]
    public class TreeView : WebControl
    {
        private TreeViewItem _Root;

        public TreeView()
            : base(HtmlTextWriterTag.Div)
        {
            _Root = new TreeViewItem();
        }

        public TreeViewItem Root { get { return _Root; } }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            using (HtmlBuilder.Start(writer, "div", new { @class = "back", style = "display: block;" }))
            {
                HtmlBuilder.Create(writer, "div", new { @class = "back-text" });
            }

            using (HtmlBuilder.Start(writer, "ul", new { @class = "tree-root" }))
            {
                AddNodes(writer, _Root);
            }
        }

        private void AddNodes(HtmlTextWriter writer, TreeViewItem item)
        {
            if (item != null)
            {
                using (HtmlBuilder.Start(writer, "li"))
                {
                    HtmlBuilder.Create(writer, "div", new { @class = "node-text" }, item.Text);
                    if (item.Count > 0)
                    {
                        foreach (var child in item)
                        {
                            using (HtmlBuilder.Start(writer, "ul", new { @class = "tree-branch" }))
                            {
                                AddNodes(writer, child);
                            }
                        }
                    }
                }
            }
        }
    }
}
