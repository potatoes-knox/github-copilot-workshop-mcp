using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Starter.CSharp.McpServer.Tests;

public sealed class McpServerE2ETests : IAsyncLifetime
{
    private const string FixedUtcNow = "2024-01-02T03:04:05.0000000Z";
    private McpClient? _client;

    public async Task InitializeAsync()
    {
        string projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../src/Starter.CSharp.McpServer/Starter.CSharp.McpServer.csproj"));

        string configuration = AppContext.BaseDirectory.Contains(
            $"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}",
            StringComparison.OrdinalIgnoreCase)
            ? "Release"
            : "Debug";

        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "starter-csharp-tests",
            Command = "dotnet",
            Arguments = ["run", "--no-build", "--configuration", configuration, "--project", projectPath],
            WorkingDirectory = Path.GetDirectoryName(projectPath),
            EnvironmentVariables = new Dictionary<string, string?>
            {
                [global::Starter.CSharp.McpServer.ToolHandlers.FixedUtcNowEnvironmentVariable] = FixedUtcNow,
            },
        });

        _client = await McpClient.CreateAsync(transport);
    }

    public async Task DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }

    [Fact]
    public async Task ListTools_ReturnsExactlyEchoAndDateTime()
    {
        IList<McpClientTool> tools = await _client!.ListToolsAsync();
        string[] names = tools.Select(tool => tool.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray();

        Assert.Equal(["dateTime", "echo"], names);
    }

    [Fact]
    public async Task ListTools_UsesStrictInputSchemas()
    {
        IList<McpClientTool> tools = await _client!.ListToolsAsync();
        JsonElement echoSchema = Assert.Single(tools.Where(tool => tool.Name == "echo")).JsonSchema;
        JsonElement dateTimeSchema = Assert.Single(tools.Where(tool => tool.Name == "dateTime")).JsonSchema;

        Assert.Equal("object", echoSchema.GetProperty("type").GetString());
        Assert.False(echoSchema.GetProperty("additionalProperties").GetBoolean());
        Assert.Equal("string", echoSchema.GetProperty("properties").GetProperty("input").GetProperty("type").GetString());
        Assert.Equal("input", Assert.Single(echoSchema.GetProperty("required").EnumerateArray()).GetString());

        Assert.Equal("object", dateTimeSchema.GetProperty("type").GetString());
        Assert.False(dateTimeSchema.GetProperty("additionalProperties").GetBoolean());
        Assert.Empty(dateTimeSchema.GetProperty("properties").EnumerateObject());
    }

    [Fact]
    public async Task Echo_ReturnsInputUnchanged()
    {
        CallToolResult result = await _client!.CallToolAsync("echo", new Dictionary<string, object?> { ["input"] = "ping" });

        string text = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Equal("ping", text);
    }

    [Fact]
    public async Task DateTime_ReturnsFixedUtcTimestampWhenConfigured()
    {
        CallToolResult result = await _client!.CallToolAsync("dateTime", new Dictionary<string, object?>());

        string text = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Equal(FixedUtcNow, text);
    }

    [Fact]
    public async Task Echo_MissingInput_ReturnsClearError()
    {
        CallToolResult result = await _client!.CallToolAsync("echo", new Dictionary<string, object?>());

        Assert.True(result.IsError);
        string errorText = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Contains("input", errorText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Echo_ExtraProperties_ReturnsClearError()
    {
        CallToolResult result = await _client!.CallToolAsync("echo", new Dictionary<string, object?> { ["input"] = "ping", ["extra"] = 1 });

        Assert.True(result.IsError);
        string errorText = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Contains("additional", errorText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Echo_NonStringInput_ReturnsClearError()
    {
        CallToolResult result = await _client!.CallToolAsync("echo", new Dictionary<string, object?> { ["input"] = 123 });

        Assert.True(result.IsError);
        string errorText = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Contains("string", errorText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DateTime_WithArguments_ReturnsClearError()
    {
        CallToolResult result = await _client!.CallToolAsync("dateTime", new Dictionary<string, object?> { ["input"] = "ping" });

        Assert.True(result.IsError);
        string errorText = Assert.Single(result.Content.OfType<TextContentBlock>()).Text;
        Assert.Contains("does not accept", errorText, StringComparison.OrdinalIgnoreCase);
    }
}
