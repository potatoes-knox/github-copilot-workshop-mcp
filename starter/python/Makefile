.PHONY: install format lint typecheck test coverage

install:
	python -m pip install -e ".[dev]"

format:
	ruff format .

lint:
	ruff check .
	ruff format --check .
	mypy src

typecheck:
	mypy src

test:
	pytest

coverage:
	pytest --cov=starter_python_mcp --cov-report=term-missing --cov-fail-under=50
