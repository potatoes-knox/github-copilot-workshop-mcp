from __future__ import annotations

from datetime import UTC, datetime

import pytest
from mcp import types

from starter_python_mcp.tool_handlers import (
    DATETIME_INVALID_INPUT_MESSAGE,
    ECHO_INVALID_INPUT_MESSAGE,
    date_time_tool_handler,
    echo_tool_handler,
    parse_date_time_input,
    parse_echo_input,
    to_iso8601_utc,
)


def extract_text(result: types.CallToolResult) -> str:
    first_content = result.content[0]
    assert isinstance(first_content, types.TextContent)
    return first_content.text


def test_parse_echo_input_returns_value() -> None:
    assert parse_echo_input({"input": "hello"}) == "hello"


def test_parse_echo_input_raises_on_invalid_input() -> None:
    with pytest.raises(ValueError, match='Invalid input for "echo"'):
        parse_echo_input({"input": 123})


def test_parse_date_time_input_raises_on_unexpected_arguments() -> None:
    with pytest.raises(ValueError, match='Invalid input for "dateTime"'):
        parse_date_time_input({"unexpected": True})


def test_echo_tool_handler_returns_input_unchanged() -> None:
    result = echo_tool_handler({"input": "echo me"})

    assert result.isError is False
    assert extract_text(result) == "echo me"


def test_echo_tool_handler_returns_clear_error() -> None:
    result = echo_tool_handler({})

    assert result.isError is True
    assert extract_text(result) == ECHO_INVALID_INPUT_MESSAGE


def test_to_iso8601_utc_normalizes_timezone() -> None:
    value = datetime(2025, 1, 1, 12, 30, 45, 120000, tzinfo=UTC)
    assert to_iso8601_utc(value) == "2025-01-01T12:30:45.120Z"


def test_date_time_tool_handler_returns_iso8601_utc() -> None:
    value = datetime(2025, 1, 1, 0, 0, 0, 0, tzinfo=UTC)
    result = date_time_tool_handler({}, now=value)

    assert result.isError is False
    assert extract_text(result) == "2025-01-01T00:00:00.000Z"


def test_date_time_tool_handler_returns_clear_error() -> None:
    result = date_time_tool_handler({"unexpected": True})

    assert result.isError is True
    assert extract_text(result) == DATETIME_INVALID_INPUT_MESSAGE
