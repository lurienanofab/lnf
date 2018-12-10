using LNF.Control;
using LNF.Repository.Control;
using System.Threading.Tasks;

namespace LNF
{
    public interface IControlService
    {
        BlockResponse GetBlockState(Block block);
        PointResponse SetPointState(Point point, bool state, uint duration);
        PointResponse Cancel(Point point);
    }
}
