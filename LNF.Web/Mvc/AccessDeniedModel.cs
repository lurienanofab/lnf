using LNF.Models.Data;

namespace LNF.Web.Mvc
{
    public class AccessDeniedModel : BaseModel
    {
        public AccessDeniedModel(ClientItem currentUser) : base(currentUser) { }

        public string ReturnUrl { get; set; }
    }
}