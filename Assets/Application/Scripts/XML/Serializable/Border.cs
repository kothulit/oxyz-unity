using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class Border
    {
        [XmlArray("Loop")]
        [XmlArrayItem("CheckPoint")]
        public CheckPoint[] Loop { get; set; }
    }
}
