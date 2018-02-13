using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LNF
{
    public class Authorization : IEnumerable
    {
        private DataTable _AppPages;
        private DataTable _Groups;

        public ClientModel CurrentUser
        {
            get { return CacheManager.Current.CurrentUser; }
        }

        public Authorization()
        {
            _AppPages = new DataTable();
            _AppPages.Columns.Add("ID", typeof(int));
            _AppPages.Columns.Add("GroupID", typeof(int));
            _AppPages.Columns.Add("FileName", typeof(string));
            _AppPages.Columns.Add("QueryString", typeof(string));
            _AppPages.Columns.Add("PageUrl", typeof(string), "FileName + QueryString");
            _AppPages.Columns.Add("ButtonText", typeof(string));
            _AppPages.Columns.Add("ToolTip", typeof(string));
            _AppPages.Columns.Add("AuthTypes", typeof(int));
            _AppPages.Columns.Add("Visible", typeof(bool));

            DataColumn pk = _AppPages.Columns["ID"];
            pk.AutoIncrement = true;
            pk.AutoIncrementSeed = 1;
            pk.AutoIncrementStep = 1;
            _AppPages.PrimaryKey = new DataColumn[] { pk };

            _Groups = new DataTable();
            _Groups.Columns.Add("GroupID", typeof(int));
            _Groups.Columns.Add("GroupName", typeof(string));
            pk = _Groups.Columns["GroupID"];
            _Groups.PrimaryKey = new DataColumn[] { pk };
        }

        public void AddGroup(int GroupID, string GroupName)
        {
            DataRow nr = _Groups.NewRow();
            nr["GroupID"] = GroupID;
            nr["GroupName"] = GroupName;
            _Groups.Rows.Add(nr);
        }

        public void AddPage(int groupId, string fileName, string buttonText, string toolTip, ClientPrivilege authTypes)
        {
            AddPage(groupId, fileName, string.Empty, buttonText, toolTip, authTypes);
        }

        public void AddPage(int groupId, string fileName, string queryString, string buttonText, string toolTip, ClientPrivilege authTypes)
        {
            //It is possible that the same page is added mulitple times (different QueryStrings or any other reason, maybe we just want two buttons, etc).
            //When this is the case, the AuthTypes for the last added duplicate AppPage will be the one used.

            DataRow nr = _AppPages.NewRow();
            nr["GroupID"] = groupId;
            nr["FileName"] = fileName;
            nr["QueryString"] = queryString;
            nr["ButtonText"] = buttonText;
            nr["ToolTip"] = toolTip;
            nr["AuthTypes"] = 0;
            nr["Visible"] = true;

            _AppPages.Rows.Add(nr);

            DataRow[] rows = _AppPages.Select(string.Format("FileName = '{0}'", fileName));
            foreach (DataRow dr in rows)
            {
                dr["AuthTypes"] = (int)authTypes;
            }
        }

        public ClientPrivilege SelectAuthTypes(string fileName)
        {
            ClientPrivilege result = 0;
            DataRow[] rows = _AppPages.Select(string.Format("FileName = '{0}'", fileName));
            if (rows.Length > 0)
            {
                //just use the first one, the AuthTypes will always be the same for duplicates (same as the last one added).
                result = (ClientPrivilege)Utility.ConvertTo(rows[0]["AuthTypes"], 0);
            }
            return result;
        }

        public void CheckAuth()
        {
            if (_AppPages == null) return;
            foreach (DataRow dr in _AppPages.Rows)
            {
                ClientPrivilege authTypes = (ClientPrivilege)Utility.ConvertTo(dr["AuthTypes"], 0);
                if (authTypes == 0)
                    dr["Visible"] = true;
                else
                    dr["Visible"] = CurrentUser.HasPriv(authTypes);
            }
        }

        public DataView SelectGroups()
        {
            return SelectGroups(string.Empty);
        }

        public DataView SelectGroups(int groupId)
        {
            return SelectGroups(string.Format("GroupID = {0}", groupId));
        }

        private DataView SelectGroups(string filter)
        {
            return new DataView(_Groups, filter, string.Empty, DataViewRowState.CurrentRows);
        }

        public List<Group> GroupsToList()
        {
            return this.GroupsToList(string.Empty);
        }

        public List<Group> GroupsToList(int groupId)
        {
            return this.GroupsToList(string.Format("GroupID = {0}", groupId));
        }

        private List<Group> GroupsToList(string filter)
        {
            List<Group> groups = new List<Group>();
            foreach (DataRowView drv in this.SelectGroups(filter))
            {
                groups.Add(new Group(drv));
            }
            return groups;
        }

        public DataView SelectAppPages()
        {
            return this.SelectAppPages(string.Empty);
        }

        public DataView SelectAppPages(int groupId)
        {
            return this.SelectAppPages(string.Format("GroupID = {0}", groupId));
        }

        public DataView SelectAppPages(bool visible)
        {
            return this.SelectAppPages(string.Format("Visible = {0}", visible));
        }

        public DataView SelectAppPages(int groupId, bool visible)
        {
            return this.SelectAppPages(string.Format("GroupID = {0} AND Visible = {1}", groupId, visible));
        }

        private DataView SelectAppPages(string filter)
        {
            return new DataView(_AppPages, filter, string.Empty, DataViewRowState.CurrentRows);
        }

        public List<AppPage> ToList()
        {
            return this.ToList(string.Empty);
        }

        public List<AppPage> ToList(int groupId)
        {
            return this.ToList(string.Format("GroupID = {0}", groupId));
        }

        public List<AppPage> ToList(bool visible)
        {
            return this.ToList(string.Format("Visible = {0}", visible));
        }

        public List<AppPage> ToList(int groupId, bool visible)
        {
            return this.ToList(string.Format("GroupID = {0} AND Visible = {1}", groupId, visible));
        }

        private List<AppPage> ToList(string filter)
        {
            List<AppPage> pages = new List<AppPage>();
            foreach (DataRowView drv in this.SelectAppPages(filter))
            {
                pages.Add(new AppPage(drv));
            }
            return pages;
        }

        public IEnumerator GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        public class Group
        {
            public int GroupID { get; }

            public string GroupName { get; }

            public Group(DataRowView drv)
            {
                GroupID = Utility.ConvertTo(drv["GroupID"], 0);
                GroupName = Utility.ConvertTo(drv["GroupName"], string.Empty);
            }
        }

        public class AppPage
        {
            public int ID { get; }

            public int GroupID { get; }

            public string FileName { get; }

            public string QueryString { get; }

            public string PageUrl { get; }

            public string ButtonText { get; }

            public string ToolTip { get; }

            public ClientPrivilege AuthTypes { get; }

            public bool Visible { get; }

            public AppPage(DataRowView drv)
            {
                ID = Utility.ConvertTo(drv["ID"], 0);
                GroupID = Utility.ConvertTo(drv["GroupID"], 0);
                FileName = Utility.ConvertTo(drv["FileName"], string.Empty);
                QueryString = Utility.ConvertTo(drv["QueryString"], string.Empty);
                PageUrl = Utility.ConvertTo(drv["PageUrl"], string.Empty);
                ButtonText = Utility.ConvertTo(drv["ButtonText"], "undefined");
                ToolTip = Utility.ConvertTo(drv["ToolTip"], string.Empty);
                AuthTypes = (ClientPrivilege)Utility.ConvertTo(drv["AuthTypes"], 0);
                Visible = Utility.ConvertTo(drv["Visible"], true);
            }
        }
    }
}
