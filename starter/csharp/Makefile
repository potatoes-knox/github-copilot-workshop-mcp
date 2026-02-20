SOLUTION := Starter.CSharp.sln
CONFIGURATION ?= Release
.RECIPEPREFIX := >

.PHONY: lint build test coverage

lint:
>dotnet format $(SOLUTION) --verify-no-changes
>dotnet build $(SOLUTION) -c $(CONFIGURATION)

build:
>dotnet build $(SOLUTION) -c $(CONFIGURATION)

test:
>dotnet test $(SOLUTION) -c $(CONFIGURATION)

coverage:
>dotnet test $(SOLUTION) -c $(CONFIGURATION) /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=50 /p:ThresholdType=line /p:ThresholdStat=total
