using System.Xml.Serialization;

namespace Oxyz.Xml.Serializable
{
    public class Parameter
    {
        private int _intValue = int.MinValue;
        private float _floatValue = float.NaN;
        private string _stringValue = string.Empty;

        [XmlAttribute("name")]
        public string Name { get; set; } = "NewParameter";

        [XmlAttribute("value")]
        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                _stringValue = value;
                int.TryParse(_stringValue, out _intValue);
                float.TryParse(_stringValue, out _floatValue);
            }
        }

        [XmlIgnore]
        public int IntValue
        {
            get { return _intValue; }
            private set { _intValue = value; }
        }

        [XmlIgnore]
        public float FloatValue
        {
            get { return _floatValue; }
            private set { _floatValue = value; }
        }
    }
}