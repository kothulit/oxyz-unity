using System.Xml.Serialization;

public class ColorRGBA
{
    [XmlAttribute("r")]
    public float R { get; set; } = 1.0f;

    [XmlAttribute("g")]
    public float G { get; set; } = 1.0f;

    [XmlAttribute("b")]
    public float B { get; set; } = 1.0f;

    [XmlAttribute("a")]
    public float A { get; set; } = 1.0f;
}
