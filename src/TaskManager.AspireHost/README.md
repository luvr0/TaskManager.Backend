# TaskManager

**TaskManager** is a lightweight **.NET 10** solution built on the [Ardalis Clean Architecture](https://github.com/ardalis/CleanArchitecture) template. It ships with clearly separated layers so new features, infrastructure concerns, or UI surfaces can be added without breaking existing behavior.

## Overview

TaskManager offers a pragmatic starting point for CRUD-style task tracking systems. The template keeps the domain model independent, application use cases orchestrated, and infrastructure details swappable, making it ideal for experimenting with new features, persistence engines, or deployment approaches.

### Why Clean Architecture?

- **Domain-centric design:** Business rules live in the `Core` project and stay decoupled from infrastructure.
- **Testability:** Each layer exposes clear seams for unit, integration, and functional tests.
- **Replaceable infrastructure:** Implementations (EF Core, email, queues, etc.) can be swapped without touching higher layers.
- **API-first mindset:** The web project exposes endpoints plus Scalar-based documentation for quick exploration.

---

## Architecture

| Project | Responsibility | Notes |
| --- | --- | --- |
| `TaskManager.Core` | Entities, value objects, domain services, domain events | Zero external dependencies to keep business rules pure. |
| `TaskManager.UseCases` | Commands, queries, application orchestration | Uses MediatR-style patterns and specifications. |
| `TaskManager.Infrastructure` | Persistence, external integrations, background services | Plug in EF Core, caching, telemetry, etc. |
| `TaskManager.Web` | FastEndpoints-based API surface and bootstrapping | Exposes `/scalar` for interactive docs. |
| `tests/*` | Unit, integration, and functional suites | Provides regression safety nets. |

---

## Key Features

- CRUD endpoints for tasks (create, list, update, delete).
- Scalar-powered API documentation at `/scalar` during development.
- Ready-to-extend Clean Architecture layout with clear separation of concerns.
- Supports Visual Studio 2022 and cross-platform `dotnet` CLI workflows.
- Hooks for persistence providers, authentication/authorization, and background jobs.

---

## Prerequisites

- **.NET 10 SDK**.
- **Visual Studio 2022** (latest preview with .NET 10 workloads) or any editor with the `dotnet` CLI.
- Git for cloning and version control.
- Docker.

---

## Getting Started

Clone the repository:

```
git clone https://github.com/luvr0/TaskManager.git
cd TaskManager
```

Restore dependencies:

```
dotnet restore
```

### Using Visual Studio

1. Open the `TaskManager.slnx` file.
2. Restore NuGet packages if Visual Studio has not done so automatically.
3. Press __F5__ for a debug run or __Ctrl+F5__ to launch without debugging.

### Using the CLI

From the repository root:

```
dotnet build --configuration Release
dotnet run --project src/TaskManager.Web/TaskManager.Web.csproj --configuration Release
```

---

## Testing

Run all tests:

```
dotnet test
```

Target specific projects:

```
dotnet test tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj
dotnet test tests/TaskManager.FunctionalTests/TaskManager.FunctionalTests.csproj
dotnet test tests/TaskManager.IntegrationTests/TaskManager.IntegrationTests.csproj
```

Use coverage collectors (example with Coverlet):

```
dotnet test /p:CollectCoverage=true /p:CoverletOutput=./coverage/
```

---

## Project Demonstration

A short video showcase:



link



---

## Roadmap

- Organize Scalar documentation into logical groups for easier endpoint discovery.
- Add Docker support (multi-stage build + compose file) for containerized workflows.
