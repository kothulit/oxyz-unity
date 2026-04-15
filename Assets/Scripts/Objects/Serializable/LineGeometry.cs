using System;
using System.Xml.Serialization;

[XmlRoot("LineGeometry")]
public class LineGeometry
{
    [XmlAttribute("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [XmlElement("Start")]
    public Point3D Start { get; set; } = new();

    [XmlElement("End")]
    public Point3D End { get; set; } = new();

    public bool IsValid()
    {
        if (Start == null || End == null) return false;
        if (Start.X == End.X &&
            Start.Y == End.Y &&
            Start.Z == End.Z) return false;
        return true;
    }
}
