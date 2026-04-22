using System;
using System.Xml.Serialization;

[XmlRoot("Element")]
public class Element
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "NewElement";

    [XmlAttribute("guid")]
    public string GUID { get; set; } = Guid.NewGuid().ToString();

    [XmlAttribute("style")]
    public string Style;
}