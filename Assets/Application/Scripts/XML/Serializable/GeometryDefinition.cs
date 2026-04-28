using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class GeometryDefinition
    {
        [XmlElement("Extrusion")]
        public List<ExtrusionGeometry> Extrusions { get; set; } = new();
    }
}
