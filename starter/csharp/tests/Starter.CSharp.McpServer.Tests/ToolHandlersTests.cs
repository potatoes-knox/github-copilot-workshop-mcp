using ModelContextProtocol.Protocol;

namespace Starter.CSharp.McpServer.Tests;

public class ToolHandlersTests
{
    [Fact]
    public void Echo_ReturnsInputUnchanged()
    {
        CallToolResult result = global::Starter.CSharp.McpServer.ToolHandlers.Echo("hello world");

        Assert.NotEqual(true, result.IsError);
        Assert.Equal("hello world", Assert.Single(result.Content.OfType<TextContentBlock>()).Text);
    }

    [Fact]
    public void Echo_ReturnsClearError_ForNullInput()
    {
        CallToolResult result = global::Starter.CSharp.McpServer.ToolHandlers.Echo(null);

        Assert.True(result.IsError);
        Assert.Contains("input", Assert.Single(result.Content.OfType<TextContentBlock>()).Text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetDateTimeUtcIso8601_UsesFixedEnvironmentOverride()
    {
        const string expected = "2024-01-02T03:04:05.0000000Z";
        string? previous = Environment.GetEnvironmentVariable(global::Starter.CSharp.McpServer.ToolHandlers.FixedUtcNowEnvironmentVariable);
        Environment.SetEnvironmentVariable(global::Starter.CSharp.McpServer.ToolHandlers.FixedUtcNowEnvironmentVariable, expected);

        try
        {
            string result = global::Starter.CSharp.McpServer.ToolHandlers.GetDateTimeUtcIso8601();
            Assert.Equal(expected, result);
        }
        finally
        {
            Environment.SetEnvironmentVariable(global::Starter.CSharp.McpServer.ToolHandlers.FixedUtcNowEnvironmentVariable, previous);
        }
    }
}
