using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Building")]
    public class BuildingElement : Element
    {
        [XmlArray("DefaultStyles")]
        [XmlArrayItem("DefaultStyle")]
        public List<DefaultStyle> Defaults { get; set; } = new();

        [XmlElement("InsertPoint")]
        public Point3D InsertPoint { get; set; } = new Point3D();

        [XmlElement("Extrusion")]
        public ExtrusionGeometry Extrusion { get; set; } = new();
    }
}
