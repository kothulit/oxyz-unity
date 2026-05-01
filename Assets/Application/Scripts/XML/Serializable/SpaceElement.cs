using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Space")]
    public class SpaceElement : Element
    {
        [XmlArray("DefaultStyles")]
        [XmlArrayItem("DefaultStyle")]
        public List<DefaultStyle> Defaults { get; set; } = null;

        [XmlAttribute("x")]
        public float InsertX { get; set; } = float.MinValue;

        [XmlAttribute("y")]
        public float InsertY { get; set; } = float.MinValue;

        [XmlAttribute("z")]
        public float InsertZ { get; set; } = float.MinValue;

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

        [XmlArray("DividingPlanes")]
        [XmlArrayItem("DividingPlane")]
        public List<DividingPlane> DividingPlanes { get; set; } = null;

        [XmlArray("DividingBoundaries")]
        [XmlArrayItem("DividingBoundary")]
        public List<DividingBoundary> DividingBoundaries { get; set; } = null;

        [XmlArray("Spaces")]
        [XmlArrayItem("Space")]
        public List<SpaceElement> Spaces { get; set; } = null;
    }
}