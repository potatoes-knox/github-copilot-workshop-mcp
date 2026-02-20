import { ErrorCode, McpError, type CallToolResult } from '@modelcontextprotocol/sdk/types.js';
import { z } from 'zod';

const echoInputSchema = z
  .object({
    input: z.string(),
  })
  .strict();

const dateTimeInputSchema = z.object({}).strict();

const toTextResult = (text: string): CallToolResult => ({
  content: [{ type: 'text', text }],
});

export const parseEchoInput = (args: unknown): { input: string } => {
  const parsed = echoInputSchema.safeParse(args);
  if (!parsed.success) {
    throw new McpError(
      ErrorCode.InvalidParams,
      'Invalid input for "echo": expected {"input": string}.',
    );
  }

  return parsed.data;
};

export const parseDateTimeInput = (args: unknown): Record<string, never> => {
  const parsed = dateTimeInputSchema.safeParse(args);
  if (!parsed.success) {
    throw new McpError(
      ErrorCode.InvalidParams,
      'Invalid input for "dateTime": this tool does not accept arguments.',
    );
  }

  return parsed.data;
};

export const echoToolHandler = (args: unknown): CallToolResult => {
  const { input } = parseEchoInput(args);
  return toTextResult(input);
};

export const dateTimeToolHandler = (args: unknown, now: Date = new Date()): CallToolResult => {
  parseDateTimeInput(args);
  return toTextResult(now.toISOString());
};
