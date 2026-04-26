using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DefaultStyle
    {
        [XmlAttribute("category")]
        public string Category { get; set; } = "";

        [XmlAttribute("style")]
        public string Style { get; set; } = "";
    }
}