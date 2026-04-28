using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DividingPlane
    {
        [XmlAttribute("elevation")]
        public float Elevation { get; set; }

        [XmlElement("InsertPoint")]
        public Point3D InsertPoint { get; set; } = new Point3D();

        [XmlArray("Regions")]
        [XmlArrayItem("Region")]
        public List<DividingRegion> Regions { get; set; } = new();
    }
}
