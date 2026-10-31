# Project Rules & Context

## Tech Stack & Architecture
- ASP.NET Core Web API with Clean Architecture / CQRS.
- Persistence: PostgreSQL using Dapper for read models, EF Core for writes.
- Caching: Redis for session states.

## Strict Rules
- Always use async/await; never use synchronous blocking (.Result, .Wait()).
- Follow the Result pattern (Result<T>) for domain returns; do not throw exceptions for logic control.
- Keep controllers thin; delegate to MediatR handlers or application services.
- Never add new NuGet packages without asking first.

## Verification Commands
- Build: dotnet build
- Test: dotnet test --no-build
