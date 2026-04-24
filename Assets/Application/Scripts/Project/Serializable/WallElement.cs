using System.Xml.Serialization;

[XmlRoot("Wall")]
public class WallElement : Element
{
    [XmlAttribute("material")] public string Material { get; set; } = "";
}
