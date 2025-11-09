using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using ReGo.RegonApi.Helpers;
using ReGo.RegonApi.Models;
using ServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using WcfCoreMtomEncoder;

namespace ReGo.RegonApi.Services
{
    public class RegonService
    {
        private static EndpointAddress endpointAddress = new EndpointAddress("https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");
        private readonly string regonApiKey;

        private readonly ILogger _logger;
        private readonly ChannelFactory<IUslugaBIRzewnPubl> _channelFactory;
        private readonly AsyncRetryPolicy _retryPolicy;

        private string? sid;

        public RegonService(IConfiguration configuration, ILogger<RegonService> logger)
        {
            regonApiKey = configuration[nameof(regonApiKey)] ?? throw new ArgumentException();
            _logger = logger;

            _channelFactory = new ChannelFactory<IUslugaBIRzewnPubl>(
                GetBindingForEndpoint(),
                endpointAddress);

            _channelFactory.Endpoint.EndpointBehaviors.Add(new SidHeaderEndpointBehavior(() => sid));

            _retryPolicy = Policy
                .Handle<CommunicationException>()
                .Or<TimeoutException>()
                .Or<ServerTooBusyException>()
                .Or<FaultException>()
                .WaitAndRetryAsync(
                retryCount: 3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, delay, attempt, _) =>
                {
                    _logger.LogWarning($"Retry ({attempt}/3): {exception.Message}. Delay {delay.TotalSeconds} s.");
                });
        }

        public async Task<Entity?> GetBusinessEntityDataAsync(ParametryWyszukiwania parametryWyszukiwania)
        {
            return await WithSession(async channel =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation("Getting business entity data for {@parametryWyszukiwania}", parametryWyszukiwania);
                    var result = await channel.DaneSzukajPodmiotyAsync(new DaneSzukajPodmiotyRequest(parametryWyszukiwania));

                    if (result?.DaneSzukajPodmiotyResult is not null)
                    {
                        var mapped = MapXmlToEntity(DeserializeXmlResponse(result.DaneSzukajPodmiotyResult));

                        _logger.LogInformation("Successfully retrieved entity data: {@entityData} from Regon API for {@parametryWyszukiwania}.",
                            parametryWyszukiwania, mapped);

                        return mapped;
                    }

                    return null;
                });
            });
        }

        private async Task<T> WithSession<T>(Func<IUslugaBIRzewnPubl, Task<T>> action)
        {
            var channel = _channelFactory.CreateChannel();

            try
            {
                sid = (await channel.ZalogujAsync(new ZalogujRequest(regonApiKey))).ZalogujResult;
                return await action(channel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Communication error.");
                throw;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(sid))
                {
                    await channel.WylogujAsync(new WylogujRequest(sid));
                }

                sid = null;

                try
                {
                    ((IClientChannel)channel).Close();
                }
                catch
                {
                    ((IClientChannel)channel).Abort();
                }
            }
        }

        private static EntityRoot DeserializeXmlResponse(string xml)
        {
            var serializer = new XmlSerializer(typeof(EntityRoot));
            using var reader = new StringReader(xml);
            return (EntityRoot)serializer.Deserialize(reader);
        }

        private static Entity MapXmlToEntity(EntityRoot xmlModel)
        {
            return new Entity
            {
                Regon = xmlModel.Entity.Regon,
                Nip = xmlModel.Entity.Nip,
                StatusNip = xmlModel.Entity.StatusNip,
                Nazwa = xmlModel.Entity.Nazwa,
                Wojewodztwo = xmlModel.Entity.Wojewodztwo,
                Powiat = xmlModel.Entity.Powiat,
                Gmina = xmlModel.Entity.Gmina,
                Miejscowosc = xmlModel.Entity.Miejscowosc,
                KodPocztowy = xmlModel.Entity.KodPocztowy,
                Ulica = xmlModel.Entity.Ulica,
                NrNieruchomosci = xmlModel.Entity.NrNieruchomosci,
                NrLokalu = xmlModel.Entity.NrLokalu,
                Typ = xmlModel.Entity.Typ,
                SilosID = xmlModel.Entity.SilosID,
                DataZakonczeniaDzialalnosci = xmlModel.Entity.DataZakonczeniaDzialalnosci,
                MiejscowoscPoczty = xmlModel.Entity.MiejscowoscPoczty
            };
        }

        private static Binding GetBindingForEndpoint()
        {
            var binding = new CustomBinding();
            binding.Elements.Add(new MtomMessageEncoderBindingElement(new TextMessageEncodingBindingElement()));
            var https = new HttpsTransportBindingElement
            {
                AllowCookies = true,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };
            binding.Elements.Add(https);
            return binding;
        }
    }
}
