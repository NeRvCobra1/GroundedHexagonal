# ADR-NET-0004 — Explicit Application Results for Expected Use-Case Outcomes

- Status: Accepted
- Scope: .NET implementation
- Decision type: Application design

---

## Context

`CraftItem` has several expected outcomes:

```text
success
recipe does not exist
inventory does not exist
ingredients are insufficient
```

These outcomes are meaningful to the use case itself.

They are not inherently HTTP concepts.

For example, `RecipeNotFound` does not intrinsically mean an HTTP `404` because another inbound adapter may be a CLI, message consumer or test.

---

## Decision

Expected use-case outcomes will be represented explicitly at the Application boundary.

For `UC-CRAFT-001`, the implementation uses:

```text
CraftItemResult
CraftItemStatus
```

with the initial statuses:

```text
Success
RecipeNotFound
InventoryNotFound
InsufficientIngredients
```

Inbound adapters translate these outcomes to their own protocol.

---

## Domain errors

Domain may use domain-specific exceptions to protect invariants or reject invalid operations.

When a domain condition represents an expected outcome of a use case, Application may translate it into an Application result.

For example:

```text
InsufficientIngredientsException
        ↓
CraftItemHandler
        ↓
CraftItemStatus.InsufficientIngredients
```

---

## Unexpected failures

Application must not catch every exception and turn it into a generic success/failure result.

Unexpected failures such as:

```text
infrastructure outage
programming defect
unexpected external adapter failure
```

must remain observable by the outer error-handling mechanisms.

---

## Why not HTTP codes in Application

Doing this would couple the use case to one inbound adapter:

```text
CraftItemHandler
    → returns 404
```

Instead:

```text
CraftItemHandler
    → RecipeNotFound

HTTP Adapter
    → chooses HTTP representation
```

A CLI or message consumer can map the same result differently.

---

## Consequences

### Positive

```text
Application remains transport independent
expected outcomes are explicit
adapters own protocol translation
tests can exercise the use case without HTTP
```

### Negative

```text
result types add code
mapping is required in inbound adapters
statuses must evolve carefully as use cases evolve
```

---

## Important clarification

Explicit result objects are a decision of this .NET implementation.

Hexagonal Architecture requires separation of concerns and boundaries, but does not prescribe this exact result pattern.
