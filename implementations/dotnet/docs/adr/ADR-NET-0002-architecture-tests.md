# ADR-NET-0002 — Architecture Tests as Executable Boundary Protection

- Status: Accepted
- Scope: .NET implementation
- Decision type: Implementation architecture and quality

---

## Context

The project documents explicit dependency rules between Domain, Application, adapters and Hosts.

Documentation alone cannot prevent a developer from accidentally adding an invalid project reference.

Because this repository is educational, it is especially important that the structure demonstrated by diagrams and README files matches the structure that actually compiles.

---

## Decision

The .NET reference implementation will include automated Architecture Tests.

The first level of these tests will inspect `.csproj` project references and verify the exact dependency graph established by `ADR-NET-0001`.

The initial graph is:

```text
Domain
    → no productive project references

Application
    → Domain

Inbound.Http
    → Application

Inbound.Worker
    → Application

Outbound.Persistence
    → Application
    → Domain

Host.Api
    → Application
    → Inbound.Http
    → Outbound.Persistence

Host.Worker
    → Application
    → Inbound.Worker
    → Outbound.Persistence
```

---

## Why start with project references

At this stage the productive assemblies contain very little code.

Rules based on namespaces, types or framework usage would either provide little value or pass vacuously because the relevant types do not yet exist.

Project-reference tests protect a real architectural boundary immediately.

---

## Future levels

Architecture Tests may later be extended to verify:

```text
namespace placement
forbidden framework dependencies
port naming conventions when useful
adapter isolation
Domain independence from infrastructure packages
Application independence from adapter types
```

Those rules will only be introduced when the codebase contains concrete structures worth protecting.

---

## No architecture library yet

The first tests use only the .NET Base Class Library and xUnit.

A specialized architecture-testing library may be introduced later if it adds clear value for type-level rules.

The library will not be allowed to define the architecture; it will only automate verification of decisions already documented elsewhere.

---

## Consequences

### Positive

```text
architectural drift is detected automatically
README diagrams and compiled structure remain aligned
invalid project references fail tests
no additional dependency is required initially
tests work even while productive assemblies are nearly empty
```

### Negative

```text
project-reference tests do not inspect type-level coupling
intentional graph changes require coordinated test updates
filesystem-based tests depend on the repository layout
```

These tradeoffs are accepted for the reference implementation.

---

## Important clarification

Architecture Tests are an enforcement mechanism.

They are not the source of architectural truth.

The source remains the documented architectural decisions and the language-neutral specification where applicable.


---

## Evolution note

`ADR-NET-0009` later introduced one additional outbound adapter project:

```text
Grounded.Hexagonal.Adapters.Outbound.Time
```

Its dependency rule is:

```text
Outbound.Time
    → Application
```

`Host.Worker` may reference it as part of composition.

The original principle of this ADR remains unchanged: each productive project represents a clear responsibility and dependencies continue pointing toward the Core.
