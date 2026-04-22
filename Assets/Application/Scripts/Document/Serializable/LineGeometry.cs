using System.Xml.Serialization;

[XmlRoot("LineGeometry")]
public class LineGeometry
{
    [XmlElement("Start")]
    public Point3D Start { get; set; }

    [XmlElement("End")]
    public Point3D End { get; set; }

    public bool IsValid()
    {
        if (Start == null || End == null) return false;
        if (Start.X == End.X &&
            Start.Y == End.Y &&
            Start.Z == End.Z) return false;
        return true;
    }
}
