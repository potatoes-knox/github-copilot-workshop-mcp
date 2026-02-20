# Python starter

Location: `starter/python`

## Run (stdio)

```bash
cd starter/python
make install
python -m starter_python_mcp
```

## Lint, tests, coverage

```bash
make -C starter/python lint
pytest starter/python/tests/unit
pytest starter/python/tests/e2e
make -C starter/python coverage
```

`make coverage` enforces a minimum 50% line coverage threshold.

## MCP Inspector quick check

```bash
cd starter/python
make install
npx @modelcontextprotocol/inspector python -m starter_python_mcp
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
