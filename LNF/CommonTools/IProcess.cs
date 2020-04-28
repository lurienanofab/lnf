using System.Data;

namespace LNF.CommonTools
{
    /// <summary>
    /// An interface for ETL (Extract, Transform, Load) processes.
    /// </summary>
    public interface IProcess<T> where T : ProcessResult
    {
        T Start();

        int DeleteExisting();

        DataTable Extract();

        DataTable Transform(DataTable dtExtract);

        int Load(DataTable dtTransform);
    }
}
