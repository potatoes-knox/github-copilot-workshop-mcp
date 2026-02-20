# C# starter

Location: `starter/csharp`

## Run (stdio)

```bash
dotnet run --project starter/csharp/src/Starter.CSharp.McpServer/Starter.CSharp.McpServer.csproj
```

## Lint, tests, coverage

```bash
make -C starter/csharp lint
make -C starter/csharp test
make -C starter/csharp coverage
```

`make coverage` enforces a minimum 50% line coverage threshold.

To run only e2e tests:

```bash
dotnet test starter/csharp/Starter.CSharp.sln -c Release --filter "FullyQualifiedName~McpServerE2ETests"
```

## MCP Inspector quick check

```bash
cd starter/csharp
npx @modelcontextprotocol/inspector dotnet run --project src/Starter.CSharp.McpServer/Starter.CSharp.McpServer.csproj
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
