using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Building")]
public class BuildingElement : Element
{
    [XmlArray("DefaultStyles")]
    [XmlArrayItem("DefaultStyle")]
    public List<DefaultStyle> Defaults = new();

    [XmlElement("InsertPoint")]
    public Point3D InsertPoint = new Point3D();

    [XmlElement("Extrusion")]
    public ExtrusionGeometry Extrusion = new();
}
