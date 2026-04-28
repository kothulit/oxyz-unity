using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class Volume
    {
        [XmlAttribute("bottom")]
        public float Bottom { get; set; }

        [XmlAttribute("top")]
        public float Top { get; set; }
    }
}
