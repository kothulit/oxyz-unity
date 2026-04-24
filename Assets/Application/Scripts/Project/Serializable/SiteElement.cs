using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Site")]
public class SiteElement : Element
{
    [XmlArray("Buildings")]
    [XmlArrayItem("Building")]
    public List<BuildingElement> Buildings = new();
}
