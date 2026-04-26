using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Wall")]
    public class WallElement : Element
    {
        [XmlAttribute("material")] public string Material { get; set; } = "";
    }
}