# eAviaSales Backend

Backend service for AviaSales-like flight search and booking scenarios.

## Architecture

- `eAviaSales.Api` - HTTP API controllers and application pipeline.
- `eAviaSales.BusinessLogic` - use-case flows and domain orchestration.
- `eAviaSales.Data` - EF Core DbContext, entities, migrations, and seed.
- `eAviaSales.Domains` - shared entities, enums, and request/response models.
- `eAviaSales.Tests` - test scaffold for critical business flows.

## Run

- `dotnet restore eAviaSales.sln`
- `dotnet build eAviaSales.sln`
- `dotnet run --project eAviaSales.Api`

## Core endpoints

- `GET /api/health`
- `POST /api/auth/login`
- `POST /api/flights/search`
- `GET /api/flights/{id}`

## Tests

- `dotnet test eAviaSales.sln`
