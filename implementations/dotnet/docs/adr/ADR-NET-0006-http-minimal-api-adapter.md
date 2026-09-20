# ADR-NET-0006 — Use ASP.NET Core Minimal APIs for the First HTTP Inbound Adapter

- Status: Accepted
- Scope: .NET implementation
- Decision type: Inbound adapter implementation

---

## Context

`UC-CRAFT-001` is now implemented through Domain, Application and concrete In-Memory outbound adapters.

The project needs an HTTP inbound adapter to demonstrate how an external protocol invokes the same Application boundary.

ASP.NET Core offers multiple ways to implement HTTP endpoints, including Controllers and Minimal APIs.

---

## Decision

The first HTTP inbound adapter will use ASP.NET Core Minimal APIs.

The endpoint will live in:

```text
Grounded.Hexagonal.Adapters.Inbound.Http
```

while application startup and dependency composition remain in:

```text
Grounded.Hexagonal.Host.Api
```

---

## Separation

The HTTP adapter owns:

```text
route
HTTP request model
HTTP response model
HTTP status mapping
translation to Application command
```

Application owns:

```text
ICraftItemUseCase
CraftItemCommand
CraftItemResult
CraftItemHandler
```

Domain owns:

```text
crafting rules
inventory invariants
recipe behavior
```

---

## Initial endpoint

```text
POST /api/crafting/items
```

The adapter maps:

```text
HTTP
    ↓
CraftItemHttpRequest
    ↓
CraftItemCommand
    ↓
ICraftItemUseCase
```

---

## Result mapping

The initial HTTP mapping is:

```text
Success
    → 200 OK

RecipeNotFound
    → 404 Not Found

InventoryNotFound
    → 404 Not Found

InsufficientIngredients
    → 409 Conflict

invalid empty identifier
    → 400 Bad Request
```

These mappings belong to HTTP and therefore remain outside Application.

---

## Why Minimal APIs

For this reference implementation they provide:

```text
small adapter surface
explicit route-to-port mapping
little framework ceremony
easy visibility of translation responsibilities
```

---

## Framework dependency

Because the HTTP adapter is a normal class library using:

```text
Microsoft.NET.Sdk
```

it explicitly references:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

The Host uses `Microsoft.NET.Sdk.Web`, which references the ASP.NET Core shared framework implicitly.

---

## Integration testing

The HTTP flow is tested through:

```text
Microsoft.AspNetCore.Mvc.Testing
WebApplicationFactory<Program>
```

Tests replace the outbound ports with seeded In-Memory adapters.

This verifies the real HTTP → Application boundary without requiring an external database or network deployment.

---

## Consequences

### Positive

```text
clear HTTP boundary
transport DTOs remain outside Application
real HTTP integration tests
simple endpoint implementation
same use case remains reusable from other inbound adapters
```

### Negative

```text
HTTP adapter now depends on ASP.NET Core
status mapping must be maintained explicitly
Minimal APIs are a .NET-specific implementation choice
```

---

## Important clarification

Minimal APIs are not Hexagonal Architecture.

They are one concrete technology used to implement an inbound adapter.

A Controller, gRPC endpoint, CLI command or message consumer could invoke the same inbound port without changing `CraftItemHandler`.
