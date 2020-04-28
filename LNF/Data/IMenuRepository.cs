using System.Collections.Generic;

namespace LNF.Data
{
    public interface IMenuRepository
    {
        IEnumerable<IMenu> GetMenuItems();
    }
}