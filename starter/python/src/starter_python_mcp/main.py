from __future__ import annotations

import asyncio
import logging

from mcp.server import NotificationOptions
from mcp.server.stdio import stdio_server

from .server import create_server

LOGGER = logging.getLogger(__name__)


def configure_logging() -> None:
    logging.basicConfig(level=logging.INFO, format="%(message)s")


async def run_server() -> int:
    server = create_server()
    try:
        async with stdio_server() as (read_stream, write_stream):
            await server.run(
                read_stream,
                write_stream,
                server.create_initialization_options(
                    notification_options=NotificationOptions(),
                    experimental_capabilities={},
                ),
            )
    except Exception as error:
        LOGGER.error("Fatal MCP server error: %s", error)
        return 1

    LOGGER.info("MCP server shutdown complete.")
    return 0


def main() -> None:
    configure_logging()
    try:
        exit_code = asyncio.run(run_server())
    except KeyboardInterrupt:
        LOGGER.info("Shutdown signal received. Stopping MCP server.")
        exit_code = 0
    raise SystemExit(exit_code)


if __name__ == "__main__":
    main()
