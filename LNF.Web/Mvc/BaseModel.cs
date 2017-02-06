using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Web.Mvc.UI;

namespace LNF.Web.Mvc
{
    public abstract class BaseModel
    {
        private bool _PermissionDenied = false;

        public string CurrentPage { get; set; }
        public string CurrentSubMenuItem { get; set; }

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

        //by default page is open to everyone
        public virtual ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        public virtual string PermissionDeniedPartial
        {
            get { return null; }
        }

        public bool PermissionDenied
        {
            get { return _PermissionDenied; }
        }

        public bool Error { get; set; }

        public string ErrorPartial { get; set; }

        public ClientModel CurrentUser
        {
            get
            {
                return CacheManager.Current.CurrentUser;
            }
        }

        public void CheckAuth()
        {
            _PermissionDenied = true;

            if (AuthTypes == 0) //null means open to everyone
                _PermissionDenied = false;
            else if (CurrentUser.HasPriv(AuthTypes))
                _PermissionDenied = false;
        }
    }
}
