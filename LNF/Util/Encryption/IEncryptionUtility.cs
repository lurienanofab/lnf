namespace LNF.Util.Encryption
{
    public interface IEncryptionUtility
    {
        string EncryptText(string text);
        string DecryptText(string text);
        string Hash(string input);
    }
}
