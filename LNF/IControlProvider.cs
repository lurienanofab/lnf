using LNF.Control;
using LNF.Repository.Control;
using System.Threading.Tasks;

namespace LNF
{
    public interface IControlProvider : IServiceTypeProvider
    {
        Task<BlockResponse> GetBlockState(Block block);
        Task<PointResponse> SetPointState(Point point, bool state, uint duration);
        Task<PointResponse> Cancel(Point point);
    }
}
