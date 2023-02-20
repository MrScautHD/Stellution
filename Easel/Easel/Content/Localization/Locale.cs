using System.Collections.Generic;
using System.Xml.Serialization;
using XmlSerializer = Easel.Data.XmlSerializer;

namespace Easel.Content.Localization;

public class Locale
{
    public string Name;

    [XmlIgnore]
    public Dictionary<string, string> Strings;

    [XmlElement(ElementName = "String")]
    public XmlLocale[] XmlStrings;

    public Locale()
    {
        Name = "Unknown";
        Strings = new Dictionary<string, string>();
    }
    
    public Locale(string name)
    {
        Name = name;

        Strings = new Dictionary<string, string>();
    }

    public string GetString(string key, params object[] format)
    {
        string text = Strings.TryGetValue(key, out string value)
            ? string.Format(value, format)
            : key;

        return text;
    }

    public string ToXml()
    {
        XmlStrings = new XmlLocale[Strings.Count];
        int i = 0;
        foreach ((string key, string value) in Strings)
        {
            XmlStrings[i].Key = key;
            XmlStrings[i++].Value = value;
        }

        return XmlSerializer.Serialize(this);
    }

    public struct XmlLocale
    {
        [XmlAttribute] public string Key;
        
        [XmlText]
        public string Value;
    }
}