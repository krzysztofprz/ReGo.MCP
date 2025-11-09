using System.Text.Json.Serialization;

namespace ReGo.RegonApi.Models
{
    [JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
    [JsonSerializable(typeof(Entity))]
    public partial class EntityGenerationContext : JsonSerializerContext
    {
    }

    public class Entity
    {
        public string Regon { get; set; }
        public string Nip { get; set; }
        public string StatusNip { get; set; }
        public string Nazwa { get; set; }
        public string Wojewodztwo { get; set; }
        public string Powiat { get; set; }
        public string Gmina { get; set; }
        public string Miejscowosc { get; set; }
        public string KodPocztowy { get; set; }
        public string Ulica { get; set; }
        public string NrNieruchomosci { get; set; }
        public string NrLokalu { get; set; }
        public string Typ { get; set; }
        public string SilosID { get; set; }
        public string DataZakonczeniaDzialalnosci { get; set; }
        public string MiejscowoscPoczty { get; set; }
    }
}
