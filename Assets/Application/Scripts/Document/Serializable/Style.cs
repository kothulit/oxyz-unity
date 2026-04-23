using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Style")]
public class Style
{
    [XmlAttribute("name")] public string Name { get; set; } 
    [XmlAttribute("guid")] public string GUID { get; set; } 
    [XmlAttribute("category")] public string Category { get; set; } = "None";

    [XmlElement("Wall", typeof(WallElement))]
    [XmlElement("Space", typeof(SpaceElement))]
    public Element Definition { get; set; }
}
