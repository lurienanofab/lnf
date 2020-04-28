namespace LNF
{
    public interface IFile
    {
        string FileName { get; set; }
        byte[] Data { get; set; }
    }
}
