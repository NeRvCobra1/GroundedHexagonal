# Grounded.Hexagonal.Adapters.Outbound.Persistence

Este proyecto contiene outbound adapters relacionados con almacenamiento y recuperación de estado requerido por Application.

Implementa ports definidos por:

```text
Grounded.Hexagonal.Application
```

---

## Responsabilidad arquitectónica

Representa:

```text
Outbound Adapter
```

para capacidades de persistencia o almacenamiento.

Application expresa qué necesita.

Este proyecto decide cómo proporcionarlo.

---

## Dependencias

Puede depender de:

```text
Application
Domain
tecnologías concretas de persistencia
```

Actualmente utiliza:

```text
Microsoft.EntityFrameworkCore.Sqlite
```

No debe depender de:

```text
Inbound Adapters
Hosts
ASP.NET Core
```

---

## Implementaciones actuales

```text
Persistence/
├── InMemory/
└── EntityFrameworkCore/
```

### InMemory

```text
InMemoryInventoryRepository
InMemoryRecipeRepository
InMemoryFoodRepository
```

Mantiene estado únicamente durante la vida del proceso.

### EntityFrameworkCore

```text
EfCoreInventoryRepository
EfCoreRecipeRepository
EfCoreFoodRepository
```

Persiste el estado mediante:

```text
EF Core
    ↓
SQLite
```

---

## El mismo port, dos adapters

Ejemplo:

```text
                    IInventoryRepository
                         ▲        ▲
                         │        │
              InMemory adapter   EF Core adapter
                                    │
                                    ▼
                                  SQLite
```

`CraftItemHandler` sólo conoce:

```text
IInventoryRepository
IRecipeRepository
```

Por eso no cambia al sustituir el mecanismo de persistencia.

---

## Modelos de persistencia

El adapter de EF Core utiliza modelos propios.

No se agregan atributos de EF a las entidades de Domain.

```text
Domain Inventory
       ↕ mapper
InventoryRecord / InventoryItemRecord
       ↕ EF Core
SQLite
```

Esto mantiene separado:

```text
modelo del negocio
```

de:

```text
modelo de almacenamiento
```

---

## Tests de integración

`Grounded.Hexagonal.Persistence.IntegrationTests` ejecuta los mismos ports contra:

```text
InMemory
SQLite real
```

Para SQLite se crea una base temporal por test.

También se ejecutan flujos reales de:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

utilizando repositories EF Core.

---

## Inicialización y migrations

El adapter administra el schema mediante EF Core Migrations:

```text
EfCoreDatabaseInitializer
    → MigrateAsync
    → Database.MigrateAsync()
```

La migración inicial vive en:

```text
EntityFrameworkCore/Migrations/
```

y crea las tablas del adapter junto con:

```text
__EFMigrationsHistory
```

`EnsureCreatedAsync()` ya no se utiliza.

---

## Qué puede contener

```text
DbContext
persistence records
mappings
SQLite configuration
EF Core repositories
database initialization
```

---

## Qué no debe contener

```text
HTTP endpoints
Controllers
Background workers
Application handlers
Domain business rules
```

---

## Regla principal

Application no sabe si sus datos vienen de memoria, SQLite, SQL Server, archivos o un servicio externo.

Los adapters absorben esas diferencias tecnológicas.


---

## Quién elige el adapter

Este proyecto ofrece implementaciones concretas, pero **no decide cuál está activa**.

La selección ocurre en cada Composition Root:

```text
Host.Api
Host.Worker
```

mediante:

```text
Persistence:Provider
```

Esto evita convertir al propio adapter de persistencia en un selector global de infraestructura.
