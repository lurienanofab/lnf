namespace LNF.Models
{
    public interface IFile
    {
        string FileName { get; set; }
        byte[] Data { get; set; }
    }
}
