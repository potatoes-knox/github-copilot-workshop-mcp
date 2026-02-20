using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using Starter.CSharp.McpServer;

var builder = Host.CreateApplicationBuilder(args);
JsonElement echoInputSchema = ToolContracts.EchoInputSchema;
JsonElement dateTimeInputSchema = ToolContracts.DateTimeInputSchema;

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithRequestFilters(filters =>
    {
        filters.AddCallToolFilter(next => async (request, cancellationToken) =>
        {
            if (!ToolContracts.TryValidateCallTool(request.Params, out CallToolResult? error))
            {
                return error!;
            }

            return await next(request, cancellationToken);
        });

        filters.AddListToolsFilter(next => async (request, cancellationToken) =>
        {
            ListToolsResult result = await next(request, cancellationToken);

            foreach (Tool tool in result.Tools)
            {
                if (string.Equals(tool.Name, "echo", StringComparison.Ordinal))
                {
                    tool.InputSchema = echoInputSchema;
                }
                else if (string.Equals(tool.Name, "dateTime", StringComparison.Ordinal))
                {
                    tool.InputSchema = dateTimeInputSchema;
                }
            }

            return result;
        });
    });

using IHost host = builder.Build();
await host.RunAsync();
