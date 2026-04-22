using System.Collections.Generic;
using System.Xml.Serialization;


[XmlRoot("Extrusion")]
public class ExtrusionGeometry
{
    [XmlAttribute("bottom")]
    public float Bottom { get; set; }

    [XmlAttribute("top")]
    public float Top { get; set; }

    [XmlArray("Loop")]
    [XmlArrayItem("CheckPoint")]
    public List<CheckPoint> Loop { get; set; } = new();
}
