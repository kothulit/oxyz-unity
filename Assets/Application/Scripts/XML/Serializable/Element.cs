using System;
using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    [XmlRoot("Element")]
    public class Element
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public bool NameSpecified { get; set; }

        [XmlAttribute("guid")]
        public string GUID { get; set; }

        [XmlIgnore]
        public bool GuidSpecified { get; set; }

        [XmlAttribute("style")]
        public string Style { get; set; }

        [XmlIgnore]
        public bool StyleSpecified { get; set; }
    }
}