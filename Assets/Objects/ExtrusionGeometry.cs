using System;
using System.Collections.Generic;
using System.Xml.Serialization;


[XmlRoot("ExtrusionGeometry")]
public class ExtrusionGeometry
{
    [XmlAttribute("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [XmlElement("InsertPoint")]
    public Point3D InsertPoint { get; set; } = new();
    [XmlElement("BottomOffset")]
    public float BottomOffset { get; set; }
    [XmlElement("TopOffset")]
    public float TopOffset { get; set; }
    [XmlArray("Contour")]
    [XmlArrayItem("Segment")]
    public List<LineSegment2D> Contour { get; set; } = new();
    public bool IsValid()
    {
        if (Contour == null || Contour.Count < 3)
            return false;
        if (TopOffset <= BottomOffset)
            return false;
        return true;
    }
}
