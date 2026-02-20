# Branch protection guidance

Configure branch protection for `main` to require pull requests and required status checks before merge.

Recommended required checks:
- `CI - C# / csharp`
- `CI - Python / python`
- `CI - TypeScript / typescript`
- `CI - Pre-commit / pre-commit`
- `CodeQL / Analyze (csharp)`
- `CodeQL / Analyze (python)`
- `CodeQL / Analyze (javascript)`
