from __future__ import annotations

from collections.abc import Mapping
from typing import Any

from mcp import types
from mcp.server import Server

from .tool_handlers import date_time_tool_handler, echo_tool_handler, text_result

SERVER_NAME = "starter-python-mcp"
SERVER_VERSION = "0.1.0"


def tool_definitions() -> list[types.Tool]:
    return [
        types.Tool(
            name="echo",
            description="Returns input unchanged.",
            inputSchema={
                "type": "object",
                "properties": {"input": {"type": "string"}},
                "required": ["input"],
                "additionalProperties": False,
            },
        ),
        types.Tool(
            name="dateTime",
            description="Returns the current date/time in ISO-8601 UTC.",
            inputSchema={
                "type": "object",
                "properties": {},
                "additionalProperties": False,
            },
        ),
    ]


def dispatch_tool_call(name: str, arguments: Mapping[str, Any] | None) -> types.CallToolResult:
    if name == "echo":
        return echo_tool_handler(arguments)
    if name == "dateTime":
        return date_time_tool_handler(arguments)
    return text_result(f'Unknown tool "{name}".', is_error=True)


def create_server() -> Server[object, object]:
    server: Server[object, object] = Server(SERVER_NAME, version=SERVER_VERSION)

    @server.list_tools()
    async def handle_list_tools() -> list[types.Tool]:
        return tool_definitions()

    @server.call_tool(validate_input=False)
    async def handle_call_tool(name: str, arguments: dict[str, Any] | None) -> types.CallToolResult:
        return dispatch_tool_call(name, arguments)

    return server
