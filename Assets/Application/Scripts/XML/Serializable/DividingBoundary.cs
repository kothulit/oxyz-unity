using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DividingBoundary
    {
        [XmlArray("Path")]
        [XmlArrayItem("CheckPoint")]
        public CheckPoint[] Path { get; set; }
    }
}
