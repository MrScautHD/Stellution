using System.IO;
using System.Xml;

namespace Easel.Data;

public static class XmlSerializer
{
    public static string Serialize(object obj)
    {
        StringWriter writer = new StringWriter();
        XmlTextWriter xmlWriter = new XmlTextWriter(writer);
        xmlWriter.Formatting = Formatting.Indented;
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
        serializer.Serialize(xmlWriter, obj);
        return writer.ToString();
    }

    public static T Deserialize<T>(string text)
    {
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        return (T) serializer.Deserialize(new StringReader(text));
    }
}