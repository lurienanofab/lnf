namespace LNF.Impl.Serialization
{
    public class SerializationService : ISerializationService
    {
        private ISerializer _Xml;
        private ISerializer _Json;

        public ISerializer Xml { get { return _Xml; } }
        public ISerializer Json { get { return _Json; } }

        public SerializationService()
        {
            _Xml = new XmlSerializer();
            _Json = new JsonSerializer();
        }
    }
}
