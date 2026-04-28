using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DividingRegion
    {
        [XmlAttribute("elevation")]
        public float Elevation { get; set; }

        [XmlArray("Loop")]
        [XmlArrayItem("CheckPoint")]
        public CheckPoint[] Loop { get; set; }
    }
}
