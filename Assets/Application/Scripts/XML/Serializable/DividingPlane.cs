using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DividingPlane
    {
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

        [XmlArray("Regions")]
        [XmlArrayItem("Region")]
        public List<DividingRegion> Regions { get; set; } = new();
    }
}
