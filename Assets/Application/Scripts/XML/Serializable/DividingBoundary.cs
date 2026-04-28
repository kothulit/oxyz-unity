using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class DividingBoundary
    {
        [XmlAttribute("shape")]
        public string Shape { get; set; } = "Piece";

        [XmlElement("CheckPoint")]
        public CheckPoint[] CheckPoints { get; set; }

        [XmlArray("Path")]
        [XmlArrayItem("CheckPoint")]
        public CheckPoint[] Path { get; set; }

        [XmlIgnore]
        public CheckPoint[] Points => CheckPoints != null && CheckPoints.Length > 0
            ? CheckPoints
            : Path;
    }
}
