using System.Xml.Serialization;

namespace ReGo.RegonApi.Models
{
    public class EntityData
    {
        [XmlElement("Regon")]
        public string Regon { get; set; }

        [XmlElement("Nip")]
        public string Nip { get; set; }

        [XmlElement("StatusNip")]
        public string StatusNip { get; set; }

        [XmlElement("Nazwa")]
        public string Nazwa { get; set; }

        [XmlElement("Wojewodztwo")]
        public string Wojewodztwo { get; set; }

        [XmlElement("Powiat")]
        public string Powiat { get; set; }

        [XmlElement("Gmina")]
        public string Gmina { get; set; }

        [XmlElement("Miejscowosc")]
        public string Miejscowosc { get; set; }

        [XmlElement("KodPocztowy")]
        public string KodPocztowy { get; set; }

        [XmlElement("Ulica")]
        public string Ulica { get; set; }

        [XmlElement("NrNieruchomosci")]
        public string NrNieruchomosci { get; set; }

        [XmlElement("NrLokalu")]
        public string NrLokalu { get; set; }

        [XmlElement("Typ")]
        public string Typ { get; set; }

        [XmlElement("SilosID")]
        public string SilosID { get; set; }

        [XmlElement("DataZakonczeniaDzialalnosci")]
        public string DataZakonczeniaDzialalnosci { get; set; }

        [XmlElement("MiejscowoscPoczty")]
        public string MiejscowoscPoczty { get; set; }
    }
}
