using System.Xml;

namespace LNF.Data
{
    public interface IClientPreference
    {
        int ClientPreferenceID { get; set; }
        int ClientID { get; set; }
        string Preferences { get; set; }
        string ApplicationName { get; set; }
        T GetPreference<T>(string name, T defval);
        XmlNode SetPreference(string name, object value);
    }
}
