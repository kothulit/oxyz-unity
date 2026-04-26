using System.IO;
using System.Xml.Serialization;
using Oxyz.Xml.Serializable;

public static class LineGeometryXmlLoader
{
    public static LineGeometry LoadFromFile(string path)
    {
        var serializer = new XmlSerializer(typeof(LineGeometry));
        using var stream = File.OpenRead(path);
        return (LineGeometry)serializer.Deserialize(stream);
    }
}
