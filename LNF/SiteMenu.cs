using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF
{
    public class SiteMenu
    {
        public ClientModel Client { get; }

        private SiteMenu(ClientModel client)
        {
            if (client == null)
                throw new ArgumentNullException("The parameter client cannot be null.");
            Client = client;
        }

        public static SiteMenu Create(ClientModel client)
        {
            return new SiteMenu(client);
        }

        public bool IsVisible(Menu item)
        {
            return item.IsVisible(Client);
        }

        public bool IsKiosk
        {
            get { return Providers.Context.Current.UserHostAddress.StartsWith("192.168.1"); }
        }

        public IList<Menu> Select()
        {
            IList<Menu> menu = DA.Current.Query<Menu>().Where(x => !x.Deleted && x.Active).ToList();
            SetLoginUrl(menu);
            return menu;

            //bool useIOF2 = this.UseIOF2;

            //DataTable dt = new DataTable();
            //dt.Columns.Add("MenuID", typeof(int));
            //dt.Columns.Add("MenuParentID", typeof(int));
            //dt.Columns.Add("MenuText", typeof(string));
            //dt.Columns.Add("MenuURL", typeof(string));
            //dt.Columns.Add("MenuCssClass", typeof(string));
            //dt.Columns.Add("MenuVisible", typeof(bool));
            //dt.Columns.Add("NewWindow", typeof(bool));
            //dt.Columns.Add("IsLogout", typeof(bool));

            //dt.Rows.Add(1, 0, "Applications", DBNull.Value, "menu-applications", true, false, false);
            //dt.Rows.Add(2, 0, "Reports", DBNull.Value, "menu-reports", true, false, false);
            //dt.Rows.Add(3, 0, "Administration", DBNull.Value, "menu-administration", _Client.HasPriv(ClientPrivilege.Administrator) || _Client.HasPriv(ClientPrivilege.Staff), false, false);
            //dt.Rows.Add(4, 0, "Help", DBNull.Value, "menu-help", true, false, false);
            //dt.Rows.Add(5, 0, "Logout", "/sselonline/Login.aspx", "menu-logout", true, false, true);

            ////Applications
            //dt.Rows.Add(6, 1, "Scheduler", "/sselScheduler/", DBNull.Value, true, false, false);
            //dt.Rows.Add(7, 1, "Store", "/sselStore/", DBNull.Value, true, false, false);
            //dt.Rows.Add(8, 1, "IOF 2.0", "/IOF/", DBNull.Value, useIOF2, false, false);
            //dt.Rows.Add(9, 1, "IOF", "/sselIOF/", DBNull.Value, !useIOF2, false, false);
            //dt.Rows.Add(10, 1, "Control", "/sselControl/", DBNull.Value, true, false, false);
            //dt.Rows.Add(11, 1, "Chemical Inventory", "/ChemicalInventoryControl/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Staff), false, false);
            //dt.Rows.Add(12, 1, "User Data", "/sselUser/", DBNull.Value, true, false, false);
            //dt.Rows.Add(13, 1, "Ext Website", "/sselExtSite/", DBNull.Value, true, false, false);
            //dt.Rows.Add(14, 1, "Feedback", "/sselFeedback/", DBNull.Value, true, false, false);
            //dt.Rows.Add(15, 1, "Mass Email", "/MassEmailer/", DBNull.Value, true, false, false);
            //dt.Rows.Add(16, 1, "Dry Box", "/DryboxManagement/", DBNull.Value, true, false, false);
            //dt.Rows.Add(17, 1, "File Storage", "/sselonline/files/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Staff), false, false);

            ////Reports
            //dt.Rows.Add(18, 2, "User Reports", "/sselIndReports/", DBNull.Value, true, false, false);
            //dt.Rows.Add(19, 2, "Resource Reports", "/sselResReports/", DBNull.Value, true, false, false);
            //dt.Rows.Add(20, 2, "Store Reports", "/sselStrReports/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Staff), false, false);

            ////Administration
            //dt.Rows.Add(21, 3, "Data Entry", "/sselData/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Administrator), false, false);
            //dt.Rows.Add(22, 3, "Fin Ops", "/sselFinOps/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Administrator), false, false);
            //dt.Rows.Add(23, 3, "Control Admin", "/sselControlAdmin/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Administrator), false, false);
            //dt.Rows.Add(24, 3, "Misc", "/sselMisc/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Staff), false, false);
            //dt.Rows.Add(25, 3, "IT Task Manager", "/TaskManager/", DBNull.Value, _Client.HasPriv(ClientPrivilege.Administrator), false, false);

            ////Help
            //dt.Rows.Add(26, 4, "General Help", "/help", DBNull.Value, true, false, false);
            //dt.Rows.Add(27, 4, "User Fees", "/help/fees", DBNull.Value, true, false, false);
            //dt.Rows.Add(28, 4, "User Committee", "/help/lnfc", DBNull.Value, true, false, false);
            //dt.Rows.Add(31, 4, "Staff Directory", "/help/staff", DBNull.Value, true, false, false);
            //dt.Rows.Add(29, 4, "Staff Calendar", "/help/calendar/staff", DBNull.Value, true, false, false);
            //dt.Rows.Add(30, 4, "Facility Calendar", "/help/calendar/facility", DBNull.Value, true, false, false);
            //dt.Rows.Add(31, 4, "Helpdesk", "/help/helpdesk", DBNull.Value, true, false, false);
            //dt.Rows.Add(32, 4, "Helpdesk", "http://lnf.umich.edu/helpdesk", DBNull.Value, true, true, false);

            //return dt;
        }

        private void SetLoginUrl(IEnumerable<Menu> items)
        {
            var logout = items.FirstOrDefault(x => x.IsLogout);
            if (logout != null)
            {
                logout.MenuURL = Providers.Context.LoginUrl;
            }
        }
    }
}
