# ADR-NET-0011 — Select Persistence Adapters in the Hosts Through Configuration

- Status: Accepted
- Scope: .NET implementation
- Decision type: Composition Root / configuration

> Schema-initialization details in this ADR are superseded by ADR-NET-0012, which replaces EnsureCreated with EF Core Migrations.

---

## Context

The implementation now has two concrete persistence strategies:

```text
InMemory
EntityFrameworkCore + SQLite
```

Application owns the ports and must remain unaware of which concrete adapter is active.

A concrete selection is nevertheless required when an executable starts.

---

## Decision

Each executable Host will select and register the persistence adapter it needs.

The selection is controlled by:

```text
Persistence:Provider
```

Supported values:

```text
InMemory
Sqlite
```

SQLite uses the standard .NET connection-string section:

```text
ConnectionStrings:Grounded
```

---

## Why the Host owns the selection

The Host is the Composition Root.

It is intentionally allowed to know both:

```text
Application port
concrete adapter
```

For example:

```text
Host.Api
    IInventoryRepository
        → InMemoryInventoryRepository

or

Host.Api
    IInventoryRepository
        → EfCoreInventoryRepository
```

Neither Domain nor Application participates in this choice.

---

## Different Hosts compose different capabilities

The API needs:

```text
IInventoryRepository
IRecipeRepository
```

The Worker needs:

```text
IFoodRepository
```

Therefore each Host registers only the concrete persistence capabilities it actually uses.

---

## SQLite initialization

When SQLite is selected, the Host creates the schema before starting normal execution:

```text
EfCoreDatabaseInitializer
    → EnsureCreatedAsync
```

Migrations remain intentionally deferred to a separate milestone.

---

## Default

The default configuration remains:

```text
InMemory
```

This preserves the fast local/demo behavior already used by the project.

Switching to SQLite does not require recompilation.

---

## Configuration sources

Because the Hosts use the standard .NET configuration pipeline, the values can come from normal configuration providers such as:

```text
appsettings.json
appsettings.{Environment}.json
environment variables
command-line arguments
```

The architectural decision is the location of the selection in the Host, not a particular provider.

---

## Fail-fast behavior

If `Sqlite` is selected without `ConnectionStrings:Grounded`, startup fails with a clear configuration error.

An unsupported provider value also fails during composition.

This is preferred over silently falling back to another persistence strategy.

---

## Consequences

### Positive

```text
Application remains adapter-independent
adapter replacement becomes visible and demonstrable
Hosts make composition decisions explicitly
runtime configuration can change persistence without recompiling Core
```

### Negative

```text
both Hosts contain small amounts of similar composition code
startup now has a persistence-initialization responsibility when SQLite is selected
configuration errors can prevent startup by design
```

The small duplication is accepted because the two executables are independent Composition Roots and currently compose different port sets.

---

## Important clarification

Configuration-driven adapter selection is not required by Hexagonal Architecture.

The architectural principle being demonstrated is:

```text
Core defines contracts
Adapters implement contracts
Composition Root chooses implementations
```
