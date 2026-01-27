using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReGo.RegonApi.Services;
using ReGo.RegonApi.Tools;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

if (string.IsNullOrEmpty(builder.Configuration.GetValue<string>("regonApiKey")))
{
    Console.Error.WriteLine("Error: REGON API KEY missing.");
    Environment.Exit(1);
}

builder.Services.AddSingleton<RegonService>();

builder.Services
    .AddMcpServer() // 1. Add MCP server
    .WithStdioServerTransport() // 2. Transport to use (stdio)
    .WithTools<RegonApiTool>(); // 3. Tool to register

// 4. Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

await builder.Build().RunAsync();
