# ADR-NET-0012 — Manage SQLite Schema Evolution with EF Core Migrations

- Status: Accepted
- Scope: .NET implementation
- Decision type: Persistence / schema evolution

---

## Context

The SQLite adapter was introduced first with:

```text
Database.EnsureCreatedAsync()
```

That was useful for isolating the initial EF Core mapping lesson.

The project now needs an explicit mechanism for evolving an existing relational schema.

`EnsureCreated` creates a schema directly from the current model and bypasses the migrations history model.

---

## Decision

SQLite schema evolution will use:

```text
EF Core Migrations
```

The adapter contains:

```text
Migrations/
    InitialCreate
    GroundedDbContextModelSnapshot
```

Runtime initialization changes from:

```text
EnsureCreatedAsync
```

to:

```text
MigrateAsync
```

through:

```text
EfCoreDatabaseInitializer
```

---

## Design-time tooling

The persistence project references:

```text
Microsoft.EntityFrameworkCore.Design
```

with:

```text
PrivateAssets = all
```

so tooling support remains an implementation concern and is not propagated to consuming projects.

The repository also pins:

```text
dotnet-ef 10.0.12
```

as a local .NET tool.

---

## Design-time DbContext creation

`GroundedDbContextDesignTimeFactory` implements:

```text
IDesignTimeDbContextFactory<GroundedDbContext>
```

This lets `dotnet ef` create the persistence DbContext without depending on normal Host runtime composition.

This factory is infrastructure tooling, not a port.

---

## Runtime behavior

When SQLite is selected by a Host:

```text
Host
  ↓
EfCoreDatabaseInitializer
  ↓
Database.MigrateAsync()
  ↓
__EFMigrationsHistory
  ↓
pending migrations only
```

The same migrations are therefore used by:

```text
Host.Api
Host.Worker
integration tests
```

---

## Existing EnsureCreated databases

A database created previously with `EnsureCreatedAsync()` has no normal migrations history.

The educational project will not attempt to silently retrofit or delete such databases.

For the current laboratory, developers should delete the old local `.db` once and let migrations recreate it.

A production system containing valuable data would require an explicit baseline or transition plan.

---

## Production note

Runtime migrations are retained for this educational implementation because they make the complete lifecycle visible and keep local execution simple.

For production environments, migration bundles or reviewed SQL scripts may be preferable when deployments require:

```text
review/approval
least-privilege runtime credentials
coordinated rollout
high availability
```

That operational choice does not change Domain or Application.

---

## Consequences

### Positive

```text
schema history becomes explicit
future schema changes are versioned
empty databases can be recreated deterministically
migration state is integration-tested
EF tooling version is reproducible
```

### Negative

```text
migration files become maintained source artifacts
model changes require a corresponding migration
old EnsureCreated databases require a one-time transition
runtime migration has production deployment tradeoffs
```

---

## Architectural clarification

Migrations are not part of Hexagonal Architecture.

They are an implementation concern of one concrete outbound persistence adapter.

The Core remains unaware of:

```text
DbContext
SQLite
Migration
__EFMigrationsHistory
dotnet-ef
```
