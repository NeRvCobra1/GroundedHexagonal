# ADR-NET-0003 — Represent Ports with C# Interfaces

- Status: Accepted
- Scope: .NET implementation
- Decision type: Implementation architecture

---

## Context

Hexagonal Architecture defines inbound and outbound ports as architectural boundaries.

The architecture does not prescribe how a programming language must represent those boundaries.

The .NET reference implementation needs a concrete, readable and testable representation.

---

## Decision

Inbound and outbound ports in the .NET implementation will initially be represented using C# interfaces.

Examples:

```text
PORT-IN-CRAFT-001
    → ICraftItemUseCase

PORT-OUT-INVENTORY-001
    → IInventoryRepository

PORT-OUT-RECIPE-001
    → IRecipeRepository
```

Inbound port interfaces and outbound port interfaces will live in:

```text
Grounded.Hexagonal.Application
```

Adapters will depend on those contracts.

---

## Inbound ports

An inbound port describes an operation exposed by Application.

A Handler may implement that interface.

Example:

```text
ICraftItemUseCase
        ↑
        │ implements
CraftItemHandler
```

---

## Outbound ports

An outbound port describes a capability required by Application.

An external adapter implements that interface.

Example:

```text
IInventoryRepository
        ↑
        │ implements
Persistence Adapter
```

---

## Why interfaces

Interfaces make the dependency direction explicit in C# and allow adapters to depend on contracts owned by Application.

They also provide a straightforward seam for unit tests.

---

## Consequences

### Positive

```text
clear dependency inversion
easy test doubles
explicit contracts
familiar C# representation
```

### Negative

```text
interfaces can be overused if created without a real boundary
developers may incorrectly equate "interface" with "port"
```

The documentation must therefore keep the architectural concept separate from its C# representation.

---

## Important clarification

This decision means:

```text
In this .NET implementation:
    Port → represented by C# interface
```

It does NOT mean:

```text
Hexagonal Architecture:
    Port = interface
```

A future Java, NestJS or other implementation may represent the same port differently while still implementing the same specification.
