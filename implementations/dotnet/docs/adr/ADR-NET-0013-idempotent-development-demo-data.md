# ADR-NET-0013 — Idempotent Development Demo Data

- Status: Accepted
- Scope: .NET implementation
- Decision type: Local development / persistence bootstrap

---

## Context

The SQLite adapter and migrations are now functional, but manually exercising the HTTP use cases requires known persisted data.

A development user should be able to:

```text
start API
query inventory
craft item
restart API
query the same persisted state
```

without manually editing SQLite.

A naïve seed that rewrites the inventory on every startup would destroy the persistence behavior the exercise is intended to demonstrate.

---

## Decision

The Development environment will use:

```text
Persistence:Provider = Sqlite
DemoData:Seed = true
```

while the base configuration remains:

```text
Persistence:Provider = InMemory
DemoData:Seed = false
```

After migrations are applied, `Host.Api` may invoke:

```text
EfCoreDemoDataSeeder.SeedIfMissingAsync()
```

The demo seeder inserts stable demo entities only when their identifiers are absent.

It never resets an existing demo inventory.

---

## Stable identifiers

The demo uses fixed GUIDs stored in:

```text
EfCoreDemoDataIds
```

This makes HTTP examples reproducible without database lookups.

---

## Why the seeder lives in the persistence adapter

The seeder writes concrete SQLite persistence state and exists only to bootstrap a concrete infrastructure implementation.

It is not:

```text
a Domain rule
an Application use case
an inbound port
an outbound port required by the Core
```

The Host decides whether to invoke it.

---

## Idempotence requirement

Given an existing demo inventory:

```text
SeedIfMissingAsync()
```

must leave its quantities unchanged.

Therefore this sequence is valid:

```text
initial seed
    ↓
CraftItem
    ↓
inventory changes
    ↓
application restart
    ↓
seed runs again
    ↓
inventory remains changed
```

---

## Consequences

### Positive

```text
manual demo is reproducible
persistent state is observable across restarts
no manual SQL is required
tests can verify restart persistence
seed does not erase user actions
```

### Negative

```text
the persistence adapter contains development-only bootstrap data
fixed IDs must remain documented
development configuration differs from base configuration
```

---

## Important clarification

Demo seeding is not part of Hexagonal Architecture.

It is a concrete local-development decision kept outside Domain and Application.
