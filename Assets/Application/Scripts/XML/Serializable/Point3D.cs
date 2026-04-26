using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class Point3D
    {
        [XmlAttribute("x")]
        public float X { get; set; }
        [XmlAttribute("y")]
        public float Y { get; set; }
        [XmlAttribute("z")]
        public float Z { get; set; }
    }
}