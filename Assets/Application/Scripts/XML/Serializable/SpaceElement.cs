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
    }
}