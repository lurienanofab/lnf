namespace LNF.Models.Mail
{
    public class Attachment : IFile
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
