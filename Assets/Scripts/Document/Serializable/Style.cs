using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Style")]
public class Style
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "NewDocument";

    [XmlAttribute("guid")]
    public string GUID { get; set; } = Guid.NewGuid().ToString();

    [XmlAttribute("category")]
    public string Category { get; set; } = "None";

    [XmlArray("Parameters")]
    [XmlArrayItem("Parameter")]
    public List<Parameter> Parameters { get; set; } = new();
}
