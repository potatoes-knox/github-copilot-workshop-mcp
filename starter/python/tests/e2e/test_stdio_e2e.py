from __future__ import annotations

import re
import sys
from collections.abc import AsyncIterator
from contextlib import asynccontextmanager
from pathlib import Path

import pytest
from mcp import ClientSession, types
from mcp.client.stdio import StdioServerParameters, stdio_client

PROJECT_ROOT = Path(__file__).resolve().parents[2]


def extract_text(result: types.CallToolResult) -> str:
    first_content = result.content[0]
    assert isinstance(first_content, types.TextContent)
    return first_content.text


@asynccontextmanager
async def connected_session() -> AsyncIterator[ClientSession]:
    server_parameters = StdioServerParameters(
        command=sys.executable,
        args=["-m", "starter_python_mcp"],
        cwd=PROJECT_ROOT,
    )

    async with (
        stdio_client(server_parameters) as (read_stream, write_stream),
        ClientSession(read_stream, write_stream) as client,
    ):
        await client.initialize()
        yield client


@pytest.mark.asyncio
async def test_list_tools_returns_exactly_echo_and_date_time() -> None:
    async with connected_session() as session:
        result = await session.list_tools()
        names = sorted(tool.name for tool in result.tools)

        assert len(result.tools) == 2
        assert names == ["dateTime", "echo"]


@pytest.mark.asyncio
async def test_echo_returns_input_unchanged() -> None:
    async with connected_session() as session:
        result = await session.call_tool("echo", {"input": "ping"})

        assert result.isError is False
        assert extract_text(result) == "ping"


@pytest.mark.asyncio
async def test_date_time_returns_iso8601_utc() -> None:
    async with connected_session() as session:
        result = await session.call_tool("dateTime", {})
        text = extract_text(result)

        assert result.isError is False
        assert re.fullmatch(r"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z", text) is not None


@pytest.mark.asyncio
async def test_invalid_input_returns_clear_errors() -> None:
    async with connected_session() as session:
        echo_result = await session.call_tool("echo", {"input": 123})
        date_time_result = await session.call_tool("dateTime", {"unexpected": True})

        assert echo_result.isError is True
        assert 'Invalid input for "echo"' in extract_text(echo_result)
        assert date_time_result.isError is True
        assert 'Invalid input for "dateTime"' in extract_text(date_time_result)
