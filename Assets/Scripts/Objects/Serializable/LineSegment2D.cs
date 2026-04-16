using System.Xml.Serialization;

public class LineSegment2D
{
    [XmlElement("Start")]
    public Point2D Start { get; set; } = new();
    [XmlElement("End")]
    public Point2D End { get; set; } = new();
}
