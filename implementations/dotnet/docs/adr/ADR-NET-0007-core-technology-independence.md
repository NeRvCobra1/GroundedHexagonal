# ADR-NET-0007 — Enforce Core Technology Independence at Assembly Level

- Status: Accepted
- Scope: .NET implementation
- Decision type: Architecture enforcement

---

## Context

The existing Architecture Tests verify productive `ProjectReference` direction.

The implementation now contains enough real code to introduce a second kind of protection.

A valid project graph alone is not sufficient to guarantee that Domain and Application remain technology-independent.

For example, a developer could add a direct framework or package dependency to the Domain project without introducing another productive `ProjectReference`.

---

## Decision

Architecture Tests will protect the Core at two additional levels:

```text
compiled assembly references
project PackageReference / FrameworkReference declarations
```

The Core consists of:

```text
Grounded.Hexagonal.Domain
Grounded.Hexagonal.Application
```

---

## Domain rule

Domain must not reference:

```text
Microsoft.AspNetCore.*
Microsoft.EntityFrameworkCore.*
Grounded.Hexagonal.Adapters.*
Grounded.Hexagonal.Host.*
```

At the current stage Domain must also declare no:

```text
PackageReference
FrameworkReference
```

---

## Application rule

Application must not reference:

```text
Microsoft.AspNetCore.*
Microsoft.EntityFrameworkCore.*
Grounded.Hexagonal.Adapters.*
Grounded.Hexagonal.Host.*
```

At the current stage Application must also declare no:

```text
PackageReference
FrameworkReference
```

Application may reference Domain.

---

## Adapter contrast

The restriction is intentionally different for adapters.

The HTTP adapter is expected to know ASP.NET Core because HTTP is the technology it adapts.

Therefore the tests explicitly verify that:

```text
Grounded.Hexagonal.Adapters.Inbound.Http
    → Microsoft.AspNetCore.*
```

and its project declares:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

The persistence adapter, however, must not acquire HTTP framework dependencies.

---

## Why this matters

The architecture aims to preserve this distinction:

```text
Core
    business + use cases

Outer layers
    technology
```

Without automated checks it is easy for convenience-driven dependencies to move technology inward over time.

---

## Why not ban every non-System dependency

The current tests intentionally focus on known architectural boundaries.

A blanket rule such as:

```text
Domain may only reference System.*
```

could become unnecessarily restrictive if a future, carefully evaluated dependency is appropriate.

Architecture Tests should protect deliberate decisions, not enforce arbitrary purity.

---

## Consequences

### Positive

```text
framework leakage is detected automatically
Domain and Application remain portable
project configuration and compiled output are both checked
adapter technology dependencies remain explicit
```

### Negative

```text
intentional future dependencies require test and ADR updates
assembly-name prefix checks are conventions that must be maintained
additional test references are needed for inspection
```

---

## Important clarification

Technology independence does not mean the entire application avoids frameworks.

It means framework knowledge remains in the parts whose responsibility requires it.

For example:

```text
HTTP Adapter
    should know ASP.NET Core

Domain
    should not
```

That asymmetry is intentional and central to the implementation.
