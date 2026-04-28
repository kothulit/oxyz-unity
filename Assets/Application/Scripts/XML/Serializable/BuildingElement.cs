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

        [XmlAttribute("x")]
        public float InsertX { get; set; }

        [XmlAttribute("y")]
        public float InsertY { get; set; }

        [XmlAttribute("z")]
        public float InsertZ { get; set; }

        [XmlIgnore]
        public Point3D InsertPoint
        {
            get => new Point3D { X = InsertX, Y = InsertY, Z = InsertZ };
            set
            {
                InsertX = value?.X ?? 0f;
                InsertY = value?.Y ?? 0f;
                InsertZ = value?.Z ?? 0f;
            }
        }

        [XmlElement("Geometry")]
        public GeometryDefinition Geometry { get; set; } = new();

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
