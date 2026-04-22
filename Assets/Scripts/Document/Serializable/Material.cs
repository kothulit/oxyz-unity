using System;
using System.Xml.Serialization;

[XmlRoot("Material")]
public class Material
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "NewMaterial";

    [XmlAttribute("guid")]
    public string GUID { get; set; } = Guid.NewGuid().ToString();

    [XmlElement("Color")]
    public ColorRGBA Color { get; set; } = new();
}