using System.ComponentModel;
using System.Globalization;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Starter.CSharp.McpServer;

[McpServerToolType]
public static class ToolHandlers
{
    public const string FixedUtcNowEnvironmentVariable = "STARTER_CSHARP_MCP_FIXED_UTC_NOW";

    [McpServerTool(Name = "echo"), Description("Returns the provided input unchanged.")]
    public static CallToolResult EchoTool([Description("Input text to echo back.")] string input) => Echo(input);

    public static CallToolResult Echo(string? input)
    {
        if (input is null)
        {
            return new CallToolResult
            {
                IsError = true,
                Content =
                [
                    new TextContentBlock
                    {
                        Text = "Invalid input: 'input' is required and must be a string.",
                    },
                ],
            };
        }

        return new CallToolResult
        {
            Content =
            [
                new TextContentBlock
                {
                    Text = input,
                },
            ],
        };
    }

    [McpServerTool(Name = "dateTime"), Description("Returns the current UTC date and time in ISO-8601 format.")]
    public static string GetDateTimeUtcIso8601() => GetCurrentUtcNow().ToString("O", CultureInfo.InvariantCulture);

    private static DateTime GetCurrentUtcNow()
    {
        string? fixedUtcNow = Environment.GetEnvironmentVariable(FixedUtcNowEnvironmentVariable);
        if (DateTime.TryParseExact(fixedUtcNow, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsed))
        {
            return parsed.Kind == DateTimeKind.Utc ? parsed : parsed.ToUniversalTime();
        }

        return DateTime.UtcNow;
    }
}
