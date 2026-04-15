using System.IO;
using System.Xml.Serialization;

public static class LineGeometryXmlLoader
{
    public static LineGeometry LoadFromFile(string path)
    {
        var serializer = new XmlSerializer(typeof(LineGeometry));
        using var stream = File.OpenRead(path);
        return (LineGeometry)serializer.Deserialize(stream);
    }
}
