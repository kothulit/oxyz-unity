using System.Collections.Generic;
using System.Xml.Serialization;
using Oxyz.Xml.Serializable;

[XmlRoot("Site")]
public class SiteElement : Element
{
    [XmlArray("Buildings")]
    [XmlArrayItem("Building")]
    public List<BuildingElement> Buildings = new();
}
