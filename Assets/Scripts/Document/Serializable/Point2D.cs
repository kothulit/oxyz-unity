using System.Xml.Serialization;

public class Point2D
{
    [XmlAttribute("x")]
    public float X { get; set; }
    [XmlAttribute("y")]
    public float Y { get; set; }
}