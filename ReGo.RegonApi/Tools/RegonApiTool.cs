using ModelContextProtocol.Server;
using ReGo.RegonApi.Services;
using ServiceReference;
using System.ComponentModel;
using System.Text.Json;

namespace ReGo.RegonApi.Tools
{
    [McpServerToolType]
    public sealed class RegonApiTool
    {
        [McpServerTool, Description("Get a business entity data for a given NIP.")]
        [McpMeta("dataSource", "https://api.stat.gov.pl/Home/RegonApi")]
        public async Task<string> GetEntityDataByNipAsync(
            RegonService regonService,
            [Description("Business entity NIP number")] string nip)
        {
            return await GetEntityDataByParametryWyszukiwaniaAsync(regonService, new ParametryWyszukiwania { Nip = nip });
        }

        [McpServerTool, Description("Get a business entity data for a given KRS.")]
        [McpMeta("dataSource", "https://api.stat.gov.pl/Home/RegonApi")]
        public async Task<string> GetEntityDataByKrsAsync(
            RegonService regonService,
            [Description("Business entity KRS number")] string krs)
        {
            return await GetEntityDataByParametryWyszukiwaniaAsync(regonService, new ParametryWyszukiwania { Krs = krs });
        }

        [McpServerTool, Description("Get a business entity data for a given REGON.")]
        [McpMeta("dataSource", "https://api.stat.gov.pl/Home/RegonApi")]
        public async Task<string> GetEntityDataByRegonAsync(
            RegonService regonService,
            [Description("Business entity REGON number")] string regon)
        {
            return await GetEntityDataByParametryWyszukiwaniaAsync(regonService, new ParametryWyszukiwania { Regon = regon });
        }

        private static async Task<string> GetEntityDataByParametryWyszukiwaniaAsync(RegonService regonService, ParametryWyszukiwania parametryWyszukiwania)
        {
            var result = await regonService.GetBusinessEntityDataAsync(parametryWyszukiwania);

            if (result is null)
            {
                return "Something went wrong.";
            }

            return JsonSerializer.Serialize(result);
        }
    }
}
