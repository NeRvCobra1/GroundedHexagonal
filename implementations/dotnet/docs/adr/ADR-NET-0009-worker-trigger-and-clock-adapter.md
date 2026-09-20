# ADR-NET-0009 — Background Worker Inbound Adapter and Explicit Clock Outbound Port

- Status: Accepted
- Scope: .NET implementation
- Decision type: Inbound/outbound adapter design

---

## Context

The first two implemented use cases are initiated through HTTP.

`ProcessFoodSpoilage` exists specifically to demonstrate that Hexagonal Architecture does not equate inbound interaction with HTTP or Presentation.

The use case is time-dependent, which introduces a second architectural question:

> Where should the current time come from?

Calling the system clock directly inside Domain or Application would make time an implicit dependency.

---

## Decision

`UC-SPOILAGE-001` will be initiated by a .NET `BackgroundService` inbound adapter:

```text
FoodSpoilageWorker
```

The worker invokes:

```text
PORT-IN-SPOILAGE-001
IProcessFoodSpoilageUseCase
```

The use case obtains the current time through:

```text
PORT-OUT-CLOCK-001
IClock
```

implemented by:

```text
Grounded.Hexagonal.Adapters.Outbound.Time.SystemClock
```

Food persistence is accessed through:

```text
IFoodRepository
```

implemented initially by:

```text
InMemoryFoodRepository
```

---

## Why a separate Time adapter project

The system clock is not persistence.

Therefore it does not belong in:

```text
Grounded.Hexagonal.Adapters.Outbound.Persistence
```

A separate project makes the responsibility explicit:

```text
Grounded.Hexagonal.Adapters.Outbound.Time
```

This also demonstrates that outbound adapters are not limited to databases.

---

## Scheduling responsibility

The Worker adapter owns:

```text
interval
run-immediately behavior
BackgroundService loop
worker-cycle logging
```

These are trigger concerns.

They are not Domain rules.

---

## Domain time rule

Domain receives:

```text
currentTime
```

as an argument.

It does not call the machine clock itself.

This makes:

```text
RULE-SPOILAGE-001
```

deterministic and unit-testable.

---

## Consequences

### Positive

```text
worker proves inbound != HTTP
time becomes explicit
tests can use a fixed clock
Domain remains deterministic
clock technology remains outside Application
outbound adapters gain a non-persistence example
```

### Negative

```text
one additional productive project
more DI composition
one additional outbound abstraction
worker scheduling requires adapter-specific configuration
```

---

## Important clarification

Neither `BackgroundService` nor `IClock` is required by Hexagonal Architecture.

They are concrete choices used by this .NET implementation to preserve the intended boundaries.


---

## Specification boundary note

La elección de `.NET BackgroundService`, `SystemClock` y la separación física en un proyecto `Adapters.Outbound.Time` es específica de .NET.

En cambio, el comportamiento observable de `RULE-SPOILAGE-001` y la necesidad de una capacidad externa para acceder a `Food` pertenecen al contrato común y deben mantenerse sincronizados con `specification/`.
