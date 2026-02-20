using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Starter.CSharp.McpServer;

internal static class ToolContracts
{
    internal static readonly JsonElement EchoInputSchema = JsonDocument.Parse(
        """
        {
          "type": "object",
          "properties": {
            "input": {
              "type": "string",
              "description": "Input text to echo back."
            }
          },
          "required": ["input"],
          "additionalProperties": false
        }
        """).RootElement.Clone();

    internal static readonly JsonElement DateTimeInputSchema = JsonDocument.Parse(
        """
        {
          "type": "object",
          "properties": {},
          "additionalProperties": false
        }
        """).RootElement.Clone();

    internal static bool TryValidateCallTool(CallToolRequestParams? parameters, out CallToolResult? error)
    {
        if (parameters is null)
        {
            error = InvalidArguments("Invalid input: missing tool parameters.");
            return false;
        }

        IDictionary<string, JsonElement>? arguments = parameters.Arguments;

        if (string.Equals(parameters.Name, "echo", StringComparison.Ordinal))
        {
            bool hasOnlyStringInput = arguments is { Count: 1 } &&
                arguments.TryGetValue("input", out JsonElement input) &&
                input.ValueKind == JsonValueKind.String;

            if (!hasOnlyStringInput)
            {
                error = InvalidArguments("Invalid input: 'input' is required and must be a string with no additional properties.");
                return false;
            }
        }
        else if (string.Equals(parameters.Name, "dateTime", StringComparison.Ordinal) &&
            arguments is { Count: > 0 })
        {
            error = InvalidArguments("Invalid input: 'dateTime' does not accept any arguments.");
            return false;
        }

        error = null;
        return true;
    }

    private static CallToolResult InvalidArguments(string message) => new()
    {
        IsError = true,
        Content =
        [
            new TextContentBlock
            {
                Text = message,
            },
        ],
    };
}
