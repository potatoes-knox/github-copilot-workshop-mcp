# C# MCP starter server

This starter uses the official `ModelContextProtocol` NuGet SDK (`0.9.0-preview.1`) and exposes exactly two stdio tools:

- `echo` - returns the input unchanged
- `dateTime` - returns current UTC time in ISO-8601 format

Both tool input schemas are strict: `echo` requires `{ "input": "..." }` with no extra properties, and `dateTime` accepts no arguments.
For deterministic testing, set `STARTER_CSHARP_MCP_FIXED_UTC_NOW` to an ISO-8601 UTC timestamp (`O` format); `dateTime` returns that value.

## Run

```bash
dotnet run --project src/Starter.CSharp.McpServer/Starter.CSharp.McpServer.csproj
```

## Commands

```bash
make lint      # format + static analysis (warnings are errors)
make test      # unit + e2e tests
make coverage  # fails if total line coverage is below 50%
```
