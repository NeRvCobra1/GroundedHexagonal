# ADR-NET-0001 — Project Boundaries as Explicit Architectural Boundaries

- Status: Accepted
- Scope: .NET implementation
- Decision type: Implementation architecture

---

## Context

The reference implementation is intended to be both executable software and educational material.

A single .NET project could technically contain Domain, Application, adapters and hosting code.

However, doing so would make architectural boundaries less visible and easier to violate accidentally.

---

## Decision

The .NET implementation will use separate `.csproj` projects for the main architectural and technological responsibilities:

```text
Grounded.Hexagonal.Domain

Grounded.Hexagonal.Application

Grounded.Hexagonal.Adapters.Inbound.Http

Grounded.Hexagonal.Adapters.Inbound.Worker

Grounded.Hexagonal.Adapters.Outbound.Persistence

Grounded.Hexagonal.Host.Api

Grounded.Hexagonal.Host.Worker
```

Tests are also separated by responsibility.

---

## Dependency direction

The intended dependency direction is:

```text
Domain
   ▲
   │
Application
   ▲
   │
Adapters
   ▲
   │
Hosts
```

More precisely:

```text
Domain
    depends on no other productive project.

Application
    may depend on Domain.

Inbound Adapters
    may depend on Application.

Outbound Adapters
    may depend on Application and Domain.

Hosts
    may depend on Application and the adapters required for composition.
```

---

## Rationale

This separation makes the following easier to understand and enforce:

```text
what belongs to Domain
what belongs to Application
what is an Adapter
what is a Host
which direction dependencies should follow
```

It also allows Architecture Tests to inspect project boundaries explicitly.

---

## Consequences

### Positive

```text
clear dependency graph
better educational value
easier architecture testing
reduced accidental coupling
easier replacement of adapters
```

### Negative

```text
more projects
more project references
slightly more setup and navigation
```

These costs are accepted because the repository prioritizes explicit architecture and educational clarity.

---

## Important clarification

Using one `.csproj` per responsibility is not a requirement of Hexagonal Architecture.

It is a deliberate decision of this .NET reference implementation.

Another implementation may preserve the same architectural boundaries using a different physical structure.

---

## Related concepts

```text
Domain
Application
Inbound Adapter
Outbound Adapter
Composition Root
Dependency Inversion
Architecture Tests
```
