import { ErrorCode, McpError } from '@modelcontextprotocol/sdk/types.js';
import { describe, expect, it } from 'vitest';

import { dateTimeToolHandler, echoToolHandler } from '../../src/toolHandlers.js';

describe('echoToolHandler', () => {
  it('returns input unchanged', () => {
    const result = echoToolHandler({ input: 'hello world' });

    expect(result).toEqual({
      content: [{ type: 'text', text: 'hello world' }],
    });
  });

  it('throws an invalid params error for invalid input', () => {
    expect(() => echoToolHandler({ input: 42 })).toThrowError(McpError);

    try {
      echoToolHandler({ input: 42 });
    } catch (error) {
      expect(error).toBeInstanceOf(McpError);
      const typedError = error as McpError;
      expect(typedError.code).toBe(ErrorCode.InvalidParams);
      expect(typedError.message).toContain('Invalid input for "echo"');
    }
  });

  it('throws an invalid params error for extra properties', () => {
    expect(() => echoToolHandler({ input: 'ok', extra: 1 })).toThrowError(McpError);
  });
});

describe('dateTimeToolHandler', () => {
  it('returns ISO-8601 UTC output', () => {
    const now = new Date('2025-01-01T00:00:00.000Z');
    const result = dateTimeToolHandler({}, now);

    expect(result).toEqual({
      content: [{ type: 'text', text: '2025-01-01T00:00:00.000Z' }],
    });
  });

  it('throws an invalid params error when arguments are provided', () => {
    expect(() => dateTimeToolHandler({ unexpected: true })).toThrowError(McpError);

    try {
      dateTimeToolHandler({ unexpected: true });
    } catch (error) {
      expect(error).toBeInstanceOf(McpError);
      const typedError = error as McpError;
      expect(typedError.code).toBe(ErrorCode.InvalidParams);
      expect(typedError.message).toContain('Invalid input for "dateTime"');
    }
  });
});
