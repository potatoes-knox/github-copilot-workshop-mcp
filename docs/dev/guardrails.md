# Dev guardrails

## Pre-commit

Install and run pre-commit hooks from the repo root:

```bash
python -m pip install pre-commit
pre-commit install
pre-commit run --all-files
```

The hooks enforce formatting/linting for each starter plus basic hygiene checks.

## Linting and static analysis

- C#:
  ```bash
  make -C starter/csharp lint
  ```
  Runs `dotnet format --verify-no-changes` plus a build with warnings-as-errors.

- Python (Ruff + mypy):
  ```bash
  make -C starter/python lint
  ```

- TypeScript (ESLint + Prettier) and typecheck:
  ```bash
  npm --prefix starter/typescript run lint
  npm --prefix starter/typescript run typecheck
  ```

## Tests and coverage gates

- C#:
  ```bash
  make -C starter/csharp test
  make -C starter/csharp coverage
  ```
  Coverage fails if line coverage drops below 50%.

- Python:
  ```bash
  pytest starter/python/tests/unit
  pytest starter/python/tests/e2e
  make -C starter/python coverage
  ```

- TypeScript:
  ```bash
  npm --prefix starter/typescript run test -- tests/unit
  npm --prefix starter/typescript run test -- tests/e2e
  npm --prefix starter/typescript run test:coverage
  ```

## CI and CodeQL

CI workflows run on every PR and on pushes to `main`:

- `CI - C#`
- `CI - Python`
- `CI - TypeScript`
- `CI - Pre-commit`
- `CodeQL`

If a workflow fails, open the run in the GitHub Actions UI to see the failing step logs.
CodeQL results are published under **Security → Code scanning alerts**.

## Dependency security

Dependabot is enabled for NuGet, pip, npm, and GitHub Actions.
Review and merge Dependabot PRs only after required checks are green.
