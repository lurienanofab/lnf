using LNF.Util.Serialization;

namespace LNF.Impl.Util.Serialization
{
    public class SerializationUtility : ISerializationUtility
    {
        public ISerializer Xml { get; }
        public ISerializer Json { get; }

        public SerializationUtility()
        {
            Xml = new XmlSerializer();
            Json = new JsonSerializer();
        }
    }
}
