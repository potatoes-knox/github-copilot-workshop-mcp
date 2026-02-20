import { Client } from '@modelcontextprotocol/sdk/client/index.js';
import { StdioClientTransport } from '@modelcontextprotocol/sdk/client/stdio.js';
import { createRequire } from 'node:module';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { afterAll, beforeAll, describe, expect, it } from 'vitest';

const require = createRequire(import.meta.url);
const tsxPath = require.resolve('tsx/cli');
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const projectRoot = path.resolve(__dirname, '../..');

type ToolResult = Awaited<ReturnType<Client['callTool']>>;

const isTextContent = (value: unknown): value is { type: 'text'; text: string } => {
  if (!value || typeof value !== 'object') {
    return false;
  }

  if (!('type' in value) || !('text' in value)) {
    return false;
  }

  return value.type === 'text' && typeof value.text === 'string';
};

const getTextFromToolResult = (result: ToolResult): string => {
  if (!('content' in result) || !Array.isArray(result.content)) {
    throw new Error('Expected callTool to return content payload.');
  }

  const [firstContent] = result.content;
  if (!isTextContent(firstContent)) {
    throw new Error('Expected first content item to be text.');
  }

  return firstContent.text;
};

describe.sequential('MCP stdio server e2e', () => {
  let client: Client;

  beforeAll(async () => {
    const transport = new StdioClientTransport({
      command: process.execPath,
      args: [tsxPath, path.join(projectRoot, 'src/index.ts')],
      cwd: projectRoot,
      stderr: 'pipe',
    });

    client = new Client({
      name: 'mcp-inspector-test-client',
      version: '1.0.0',
    });

    await client.connect(transport);
  });

  afterAll(async () => {
    await client.close();
  });

  it('exposes exactly the expected tools', async () => {
    const response = await client.listTools();
    const names = response.tools.map((tool) => tool.name).sort();
    const echo = response.tools.find((tool) => tool.name === 'echo');
    const dateTime = response.tools.find((tool) => tool.name === 'dateTime');

    expect(response.tools).toHaveLength(2);
    expect(names).toEqual(['dateTime', 'echo']);
    expect(echo?.inputSchema).toMatchObject({
      type: 'object',
      properties: {
        input: {
          type: 'string',
        },
      },
      required: ['input'],
      additionalProperties: false,
    });
    expect(dateTime?.inputSchema).toMatchObject({
      type: 'object',
      properties: {},
      additionalProperties: false,
    });
  });

  it('echo returns input unchanged', async () => {
    const response = await client.callTool({
      name: 'echo',
      arguments: { input: 'echo me' },
    });

    expect(response.isError).not.toBe(true);
    expect(getTextFromToolResult(response)).toBe('echo me');
  });

  it('dateTime returns ISO-8601 UTC', async () => {
    const response = await client.callTool({
      name: 'dateTime',
      arguments: {},
    });

    const text = getTextFromToolResult(response);
    expect(response.isError).not.toBe(true);
    expect(text).toMatch(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z$/);
  });

  it('returns clear errors for invalid input', async () => {
    const echoResponse = await client.callTool({
      name: 'echo',
      arguments: { input: 123 },
    });

    expect(echoResponse.isError).toBe(true);
    expect(getTextFromToolResult(echoResponse)).toContain('Invalid arguments for tool echo');

    const dateTimeResponse = await client.callTool({
      name: 'dateTime',
      arguments: { unexpected: true },
    });

    const echoExtraResponse = await client.callTool({
      name: 'echo',
      arguments: { input: 'ok', extra: 1 },
    });

    expect(dateTimeResponse.isError).toBe(true);
    expect(getTextFromToolResult(dateTimeResponse)).toContain(
      'Invalid arguments for tool dateTime',
    );
    expect(echoExtraResponse.isError).toBe(true);
    expect(getTextFromToolResult(echoExtraResponse)).toContain('Invalid arguments for tool echo');
  });
});
