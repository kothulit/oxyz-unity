using System.IO;
using System.Xml.Serialization;
public static class ExtrusionGeometryXmlLoader
{
    public static ExtrusionGeometry LoadFromFile(string path)
    {
        var serializer = new XmlSerializer(typeof(ExtrusionGeometry));
        using var stream = File.OpenRead(path);
        return (ExtrusionGeometry)serializer.Deserialize(stream);
    }
}