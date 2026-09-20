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
```

No debe depender de:

```text
Inbound Adapters
Hosts
```

---

## Implementaciones actuales

```text
InMemory/
```

contiene:

```text
InMemoryInventoryRepository
    → PORT-OUT-INVENTORY-001

InMemoryRecipeRepository
    → PORT-OUT-RECIPE-001
```

---

## Evolución prevista

La organización permite agregar posteriormente otras implementaciones:

```text
Persistence/
├── InMemory/
└── EntityFrameworkCore/
```

Ambas podrían implementar los mismos ports.

Por ejemplo:

```text
IInventoryRepository
      ▲              ▲
      │              │
InMemory...     EfCore...
```

`CraftItemHandler` no tendría que cambiar.

---

## Repository

En esta implementación existe la separación:

```text
Application
    IInventoryRepository
        ↓
        outbound port

Persistence
    InMemoryInventoryRepository
        ↓
        outbound adapter
```

El contrato pertenece al núcleo.

La implementación tecnológica pertenece al exterior.

---

## Qué puede contener

Dependiendo de la implementación concreta:

```text
in-memory stores
EF Core
DbContext
persistence models
mappings
SQL-specific configuration
repository implementations
```

---

## Qué no debe contener

```text
HTTP endpoints
Controllers
Background workers
Application use cases
Domain business rules
```

---

## Regla principal

Application no sabe si sus datos vienen de memoria, SQL, archivos o un servicio externo.

Los adapters hacen que esas diferencias tecnológicas queden fuera del núcleo.
