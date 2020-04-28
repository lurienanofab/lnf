namespace LNF.Util.Serialization
{
    public interface ISerializationUtility
    {
        ISerializer Xml { get; }
        ISerializer Json { get; }
    }
}
