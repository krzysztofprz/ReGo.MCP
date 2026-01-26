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

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);

builder.Services.AddSingleton<RegonService>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RegonApiTool>();

await builder.Build().RunAsync();
