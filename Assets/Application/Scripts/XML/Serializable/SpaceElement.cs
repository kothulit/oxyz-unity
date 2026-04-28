using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Space")]
    public class SpaceElement : Element
    {
        [XmlArray("DefaultStyles")]
        [XmlArrayItem("DefaultStyle")]
        public List<DefaultStyle> Defaults { get; set; } = new();

        [XmlElement("InsertPoint")]
        public Point3D InsertPoint { get; set; } = new Point3D();

        [XmlArray("DividingPlanes")]
        [XmlArrayItem("DividingPlane")]
        public List<DividingPlane> DividingPlanes { get; set; } = new();

        [XmlArray("DividingBoundaries")]
        [XmlArrayItem("DividingBoundary")]
        public List<DividingBoundary> DividingBoundaries { get; set; } = new();

        [XmlArray("Spaces")]
        [XmlArrayItem("Space")]
        public List<SpaceElement> Spaces { get; set; } = new();
    }
}