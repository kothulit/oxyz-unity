using System.Xml.Serialization;

public class DefaultStyle
{
    [XmlAttribute("category")]
    public string Category { get; set; } = "";

    [XmlAttribute("style")]
    public string Style { get; set; } = "";
}
