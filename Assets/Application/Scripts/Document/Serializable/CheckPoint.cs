using System.Xml.Serialization;

public class CheckPoint
{
    [XmlAttribute("x")]
    public float X { get; set; }

    [XmlAttribute("y")]
    public float Y { get; set; }

    [XmlAttribute("style")]
    public string Style { get; set; } = "";
}
