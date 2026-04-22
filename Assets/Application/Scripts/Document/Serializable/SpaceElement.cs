using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Space")]
public class SpaceElement : Element
{
    [XmlArray("DefaultStyles")]
    [XmlArrayItem("DefaultStyle")]
    public List<DefaultStyle> Defaults = new();

    [XmlElement("InsertPoint")]
    public Point3D InsertPoint = new Point3D();
}