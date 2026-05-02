using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Space")]
    public class SpaceElement : Element
    {
        [XmlArray("DefaultStyles")]
        [XmlArrayItem("DefaultStyle")]
        public List<DefaultStyle> Defaults { get; set; }

        [XmlAttribute("x")]
        public float InsertX { get; set; }
        [XmlIgnore]
        public bool InsertXSpecified { get; set; }
        [XmlAttribute("y")]
        public float InsertY { get; set; }
        [XmlIgnore]
        public bool InsertYSpecified { get; set; }
        [XmlAttribute("z")]
        public float InsertZ { get; set; }
        [XmlIgnore]
        public bool InsertZSpecified { get; set; }

        [XmlIgnore]
        public Point3D InsertPoint
        {
            get => new Point3D { X = InsertX, Y = InsertY, Z = InsertZ };
            set
            {
                InsertX = value?.X ?? 0.0f;
                InsertY = value?.Y ?? 0.0f;
                InsertZ = value?.Z ?? 0.0f;
            }
        }

        [XmlArray("DividingPlanes")]
        [XmlArrayItem("DividingPlane")]
        public List<DividingPlane> DividingPlanes { get; set; }

        [XmlArray("DividingBoundaries")]
        [XmlArrayItem("DividingBoundary")]
        public List<DividingBoundary> DividingBoundaries { get; set; }

        [XmlArray("Spaces")]
        [XmlArrayItem("Space")]
        public List<SpaceElement> Spaces { get; set; }
    }
}