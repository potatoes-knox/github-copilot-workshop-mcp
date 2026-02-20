from __future__ import annotations

from mcp import types

from starter_python_mcp.server import create_server, dispatch_tool_call, tool_definitions


def extract_text(result: types.CallToolResult) -> str:
    first_content = result.content[0]
    assert isinstance(first_content, types.TextContent)
    return first_content.text


def test_tool_definitions_expose_exactly_echo_and_date_time() -> None:
    tools = tool_definitions()
    names = sorted(tool.name for tool in tools)

    assert names == ["dateTime", "echo"]
    assert len(tools) == 2


def test_dispatch_tool_call_returns_unknown_tool_error() -> None:
    result = dispatch_tool_call("unknown", {})

    assert result.isError is True
    assert extract_text(result) == 'Unknown tool "unknown".'


def test_dispatch_tool_call_routes_echo_handler() -> None:
    result = dispatch_tool_call("echo", {"input": "ping"})

    assert result.isError is False
    assert extract_text(result) == "ping"


def test_create_server_registers_tool_handlers() -> None:
    server = create_server()

    assert types.ListToolsRequest in server.request_handlers
    assert types.CallToolRequest in server.request_handlers
