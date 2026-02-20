from __future__ import annotations

from collections.abc import Mapping
from datetime import UTC, datetime
from typing import Any

from mcp import types

ECHO_INVALID_INPUT_MESSAGE = 'Invalid input for "echo": expected {"input": string}.'
DATETIME_INVALID_INPUT_MESSAGE = (
    'Invalid input for "dateTime": this tool does not accept arguments.'
)


def text_result(text: str, *, is_error: bool = False) -> types.CallToolResult:
    return types.CallToolResult(
        content=[types.TextContent(type="text", text=text)],
        isError=is_error,
    )


def parse_echo_input(arguments: Mapping[str, Any] | None) -> str:
    payload = {} if arguments is None else dict(arguments)
    if set(payload) != {"input"}:
        raise ValueError(ECHO_INVALID_INPUT_MESSAGE)

    value = payload["input"]
    if not isinstance(value, str):
        raise ValueError(ECHO_INVALID_INPUT_MESSAGE)

    return value


def parse_date_time_input(arguments: Mapping[str, Any] | None) -> None:
    payload = {} if arguments is None else dict(arguments)
    if payload:
        raise ValueError(DATETIME_INVALID_INPUT_MESSAGE)


def to_iso8601_utc(value: datetime) -> str:
    normalized = value.replace(tzinfo=UTC) if value.tzinfo is None else value.astimezone(UTC)
    return normalized.isoformat(timespec="milliseconds").replace("+00:00", "Z")


def echo_tool_handler(arguments: Mapping[str, Any] | None) -> types.CallToolResult:
    try:
        return text_result(parse_echo_input(arguments))
    except ValueError as error:
        return text_result(str(error), is_error=True)


def date_time_tool_handler(
    arguments: Mapping[str, Any] | None,
    *,
    now: datetime | None = None,
) -> types.CallToolResult:
    try:
        parse_date_time_input(arguments)
    except ValueError as error:
        return text_result(str(error), is_error=True)

    current_time = datetime.now(UTC) if now is None else now
    return text_result(to_iso8601_utc(current_time))
