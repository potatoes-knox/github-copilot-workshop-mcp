import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { fileURLToPath } from 'node:url';

import { createServer } from './server.js';

export const startServer = async (): Promise<void> => {
  const server = createServer();
  const transport = new StdioServerTransport();
  let shuttingDown = false;

  const shutdown = async (exitCode: number): Promise<void> => {
    if (shuttingDown) {
      return;
    }

    shuttingDown = true;

    try {
      await server.close();
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      console.error(`Failed to close MCP server cleanly: ${message}`);
    }

    process.exit(exitCode);
  };

  const handleFatalError = (error: unknown): void => {
    const message = error instanceof Error ? error.message : String(error);
    console.error(`Fatal MCP server error: ${message}`);
    void shutdown(1);
  };

  process.once('SIGINT', () => {
    void shutdown(0);
  });

  process.once('SIGTERM', () => {
    void shutdown(0);
  });

  process.once('uncaughtException', (error) => {
    handleFatalError(error);
  });

  process.once('unhandledRejection', (reason) => {
    handleFatalError(reason);
  });

  transport.onclose = () => {
    void shutdown(0);
  };

  transport.onerror = (error) => {
    handleFatalError(error);
  };

  try {
    await server.connect(transport);
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    console.error(`Failed to start MCP server: ${message}`);
    await shutdown(1);
  }
};

if (process.argv[1] && fileURLToPath(import.meta.url) === process.argv[1]) {
  void startServer();
}
