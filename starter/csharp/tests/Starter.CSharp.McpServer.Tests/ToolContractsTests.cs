using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Starter.CSharp.McpServer.Tests;

public sealed class ToolContractsTests
{
    [Fact]
    public void TryValidateCallTool_ReturnsError_WhenParametersMissing()
    {
        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(null, out CallToolResult? error);

        Assert.False(valid);
        Assert.NotNull(error);
        Assert.True(error!.IsError);
    }

    [Fact]
    public void TryValidateCallTool_AllowsEchoWithString()
    {
        CallToolRequestParams parameters = CreateParams("echo", """{"input":"hello"}""");

        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(parameters, out CallToolResult? error);

        Assert.True(valid);
        Assert.Null(error);
    }

    [Fact]
    public void TryValidateCallTool_RejectsEchoWithExtraProperties()
    {
        CallToolRequestParams parameters = CreateParams("echo", """{"input":"hello","extra":1}""");

        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(parameters, out CallToolResult? error);

        Assert.False(valid);
        Assert.NotNull(error);
        Assert.Contains("additional", GetErrorText(error!));
    }

    [Fact]
    public void TryValidateCallTool_RejectsEchoWithNonString()
    {
        CallToolRequestParams parameters = CreateParams("echo", """{"input":123}""");

        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(parameters, out CallToolResult? error);

        Assert.False(valid);
        Assert.NotNull(error);
        Assert.Contains("string", GetErrorText(error!));
    }

    [Fact]
    public void TryValidateCallTool_AllowsDateTimeWithNoArguments()
    {
        CallToolRequestParams parameters = new()
        {
            Name = "dateTime",
            Arguments = new Dictionary<string, JsonElement>(),
        };

        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(parameters, out CallToolResult? error);

        Assert.True(valid);
        Assert.Null(error);
    }

    [Fact]
    public void TryValidateCallTool_RejectsDateTimeWithArguments()
    {
        CallToolRequestParams parameters = CreateParams("dateTime", """{"unexpected":true}""");

        bool valid = global::Starter.CSharp.McpServer.ToolContracts.TryValidateCallTool(parameters, out CallToolResult? error);

        Assert.False(valid);
        Assert.NotNull(error);
        Assert.Contains("does not accept", GetErrorText(error!));
    }

    [Fact]
    public void Schemas_AreStrict()
    {
        JsonElement echoSchema = global::Starter.CSharp.McpServer.ToolContracts.EchoInputSchema;
        JsonElement dateTimeSchema = global::Starter.CSharp.McpServer.ToolContracts.DateTimeInputSchema;

        Assert.False(echoSchema.GetProperty("additionalProperties").GetBoolean());
        Assert.Equal("input", Assert.Single(echoSchema.GetProperty("required").EnumerateArray()).GetString());

        Assert.False(dateTimeSchema.GetProperty("additionalProperties").GetBoolean());
        Assert.Empty(dateTimeSchema.GetProperty("properties").EnumerateObject());
    }

    private static CallToolRequestParams CreateParams(string name, string json)
    {
        return new CallToolRequestParams
        {
            Name = name,
            Arguments = ParseArguments(json),
        };
    }

    private static Dictionary<string, JsonElement> ParseArguments(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        Dictionary<string, JsonElement> arguments = new(StringComparer.Ordinal);

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            arguments[property.Name] = property.Value.Clone();
        }

        return arguments;
    }

    private static string GetErrorText(CallToolResult result)
    {
        TextContentBlock content = Assert.Single(result.Content.OfType<TextContentBlock>());
        return content.Text;
    }
}
