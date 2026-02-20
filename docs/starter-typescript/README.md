# TypeScript starter

Location: `starter/typescript`

## Run (stdio)

```bash
cd starter/typescript
npm ci
npm run dev
```

## Lint, tests, coverage

```bash
npm --prefix starter/typescript run lint
npm --prefix starter/typescript run typecheck
npm --prefix starter/typescript run test -- tests/unit
npm --prefix starter/typescript run test -- tests/e2e
npm --prefix starter/typescript run test:coverage
```

`npm run test:coverage` enforces a minimum 50% line coverage threshold.

## MCP Inspector quick check

```bash
cd starter/typescript
npm ci
npx @modelcontextprotocol/inspector npm run dev
```

In MCP Inspector:

1. Connect to the stdio server.
2. List tools and confirm `echo` and `dateTime`.
3. Invoke `echo` with arguments:
   ```json
   {"input":"hello"}
   ```
4. Invoke `dateTime` with arguments:
   ```json
   {}
   ```
5. Invalid-input example: invoke `echo` with:
   ```json
   {"input":123}
   ```
