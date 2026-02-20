import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';

import { dateTimeToolHandler, echoToolHandler } from './toolHandlers.js';

export const createServer = (): McpServer => {
  const server = new McpServer({
    name: 'starter-typescript-mcp',
    version: '0.1.0',
  });

  server.registerTool(
    'echo',
    {
      description: 'Returns input unchanged.',
      inputSchema: z
        .object({
          input: z.string(),
        })
        .strict(),
    },
    (args) => echoToolHandler(args),
  );

  server.registerTool(
    'dateTime',
    {
      description: 'Returns the current date/time in ISO-8601 UTC.',
      inputSchema: z.strictObject({}),
    },
    (args) => dateTimeToolHandler(args),
  );

  return server;
};
