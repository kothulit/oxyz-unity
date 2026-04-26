using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Document")]
    public class Document
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = "NewDocument";

        [XmlAttribute("guid")]
        public string GUID { get; set; } = Guid.NewGuid().ToString();

        [XmlArray("DefaultStyles")]
        [XmlArrayItem("DefaultStyle")]
        public List<DefaultStyle> Defaults = new();

        [XmlElement("Site")]
        public SiteElement Site = new();
    }
}