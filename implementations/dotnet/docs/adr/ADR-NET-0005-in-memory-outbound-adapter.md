# ADR-NET-0005 — Introduce In-Memory Outbound Adapters Before Database Infrastructure

- Status: Accepted
- Scope: .NET implementation
- Decision type: Adapter implementation strategy

---

## Context

`UC-CRAFT-001` now depends on two outbound ports:

```text
PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
```

The project needs concrete implementations in order to exercise the use case beyond unit tests.

Introducing EF Core and a relational database immediately would add several concerns at once:

```text
ORM mapping
database configuration
migrations
connection management
database integration testing
```

Those concerns are valuable later, but they would obscure the architectural lesson currently being introduced.

---

## Decision

The first concrete outbound implementations will store state in process memory:

```text
InMemoryInventoryRepository
InMemoryRecipeRepository
```

They live in:

```text
Grounded.Hexagonal.Adapters.Outbound.Persistence
```

and implement contracts owned by Application.

---

## Dependency direction

```text
Application
    │
    │ owns contracts
    ▼
IInventoryRepository
IRecipeRepository
    ▲
    │ implements
    │
InMemory outbound adapters
```

Application does not reference the adapter project.

---

## Purpose

These adapters provide a simple concrete environment for:

```text
integration testing
local composition
demonstrating dependency inversion
demonstrating adapter replacement
```

They are not intended to model every behavior of a relational database.

---

## Future replacement

A later implementation may introduce:

```text
EntityFrameworkCoreInventoryRepository
EntityFrameworkCoreRecipeRepository
```

or another persistence mechanism.

The expected architectural result is:

```text
CraftItemHandler
    unchanged

Domain
    unchanged

Ports
    unchanged unless the actual capability changes
```

Only composition and the concrete adapter should need to change.

---

## Consequences

### Positive

```text
minimal infrastructure noise
fast integration tests
clear demonstration of ports/adapters
no database dependency yet
easy future comparison with EF Core
```

### Negative

```text
no durability
different materialization semantics from a database
shared object references are possible
does not model database transactions
```

These limitations are acceptable because this adapter is educational and local, not a production persistence strategy.

---

## Important clarification

In-memory persistence is not part of Hexagonal Architecture.

It is one concrete outbound adapter chosen by this .NET implementation.
