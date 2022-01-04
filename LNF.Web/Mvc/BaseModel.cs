using LNF.Data;
using LNF.DataAccess;
using LNF.Web.Mvc.UI;

namespace LNF.Web.Mvc
{
    public abstract class BaseModel
    {
        public IProvider Provider { get; set; }
        public ISession DataSession => Provider.DataAccess.Session;

        public string CurrentPage { get; set; }

        public string CurrentSubMenuItem { get; set; }

        //by default page is open to everyone
        public virtual ClientPrivilege AuthTypes => 0;

        public virtual string PermissionDeniedPartial => null;

        public bool PermissionDenied { get; private set; } = false;

        public bool Error { get; set; }

        public string ErrorPartial { get; set; }

        public IClient CurrentUser { get; set; }

        public virtual SubMenu GetSubMenu()
        {
            return SubMenu.Create(new SubMenu.MenuItem[]
            {
                new SubMenu.MenuItem()
                {
                    LinkText = "Home",
                    Active = true,
                    ActionName = "Index",
                    ControllerName = "Home"
                }
            });
        }

        public void CheckAuth()
        {
            PermissionDenied = true;

            if (AuthTypes == 0) //null means open to everyone
                PermissionDenied = false;
            else if (CurrentUser.HasPriv(AuthTypes))
                PermissionDenied = false;
        }
    }
}
