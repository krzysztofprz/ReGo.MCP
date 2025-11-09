using System.Xml.Serialization;

namespace ReGo.RegonApi.Models
{
    [XmlRoot("root")]
    public class EntityRoot
    {
        [XmlElement("dane")]
        public EntityData? Entity { get; set; }
    }
}
