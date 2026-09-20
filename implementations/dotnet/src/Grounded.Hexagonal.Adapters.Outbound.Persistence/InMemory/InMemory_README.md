# InMemory — Outbound Persistence Adapter

Esta carpeta contiene implementaciones In-Memory de outbound ports definidos por Application.

No representa una nueva capa arquitectónica.

Representa una tecnología concreta utilizada por un outbound adapter.

---

## Implementaciones

```text
InMemoryInventoryRepository
    implements
    IInventoryRepository

InMemoryRecipeRepository
    implements
    IRecipeRepository
```

---

## Flujo de dependencias

```text
Application
    │
    │ owns
    ▼
IInventoryRepository
IRecipeRepository
    ▲
    │ implements
    │
InMemory Adapter
```

Application no depende de esta carpeta.

Esta carpeta depende de los contratos definidos por Application.

---

## Por qué In-Memory primero

La implementación In-Memory permite comprobar el flujo completo sin introducir todavía:

```text
EF Core
SQL
migrations
database configuration
connection strings
```

Esto nos permite estudiar primero la frontera hexagonal.

Después podrá agregarse otro adapter, por ejemplo:

```text
EntityFrameworkCore/
```

sin modificar `CraftItemHandler`.

---

## Persistencia real

"In-Memory" es deliberadamente una implementación simple.

Su estado existe únicamente durante la vida del proceso.

No pretende simular todas las propiedades de una base de datos real:

```text
transactions
concurrency control de base de datos
durability
relational mapping
```

Es un adapter válido para pruebas, demos y composición local.

---

## Importante: referencias de objetos

Esta implementación conserva aggregates como objetos en memoria.

Por lo tanto, el objeto recuperado y el almacenado pueden compartir la misma referencia.

Esto es aceptado conscientemente para este adapter educativo.

Un adapter de base de datos posterior tendrá semánticas distintas de materialización y persistencia.

---

## Regla principal

El port define:

> qué capacidad necesita Application.

Este adapter define:

> cómo proporcionar esa capacidad usando memoria del proceso.
