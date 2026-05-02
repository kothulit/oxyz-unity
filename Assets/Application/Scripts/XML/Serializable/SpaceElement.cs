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
        public float? InsertX { get; set; }

        [XmlAttribute("y")]
        public float? InsertY { get; set; }

        [XmlAttribute("z")]
        public float? InsertZ { get; set; }

        [XmlIgnore]
        public Point3D InsertPoint
        {
            get
            {
                if (InsertX != null && InsertY != null && InsertZ == null)
                    return new Point3D { X = InsertX.Value, Y = InsertY.Value, Z = .0f };
                if (InsertX != null && InsertY != null && InsertZ != null)
                    return new Point3D { X = InsertX.Value, Y = InsertY.Value, Z = InsertZ.Value };
                return null;
            }
            set
            {
                InsertX = value?.X ?? 0f;
                InsertY = value?.Y ?? 0f;
                InsertZ = value?.Z ?? 0f;
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