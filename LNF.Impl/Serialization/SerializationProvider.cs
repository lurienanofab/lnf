namespace LNF.Impl.Serialization
{
    public class SerializationProvider : ISerializationProvider
    {
        private ISerializer _Xml;
        private ISerializer _Json;

        public ISerializer Xml { get { return _Xml; } }
        public ISerializer Json { get { return _Json; } }

        public SerializationProvider()
        {
            _Xml = new XmlSerializer();
            _Json = new JsonSerializer();
        }

        public void Dispose()
        {

        }
    }
}
