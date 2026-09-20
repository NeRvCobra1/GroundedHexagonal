# Grounded.Hexagonal.Host.Api

Este proyecto es el ejecutable que inicia la aplicación HTTP.

Actúa como Composition Root para el flujo HTTP.

---

## Responsabilidad

El Host puede encargarse de:

```text
ASP.NET Core startup
Dependency Injection
configuration
logging
middleware
registrar adapters
conectar ports con implementaciones
```

---

## Composition Root

Aquí pueden conocerse simultáneamente abstracciones e implementaciones concretas.

Ejemplo:

```text
IInventoryRepository
        ↓
InventoryRepository
```

El Host realiza la conexión.

Application no conoce esa implementación concreta.

---

## Puede depender de

```text
Application
Inbound.Http
Outbound.Persistence
```

---

## No debe contener

```text
reglas de negocio
casos de uso
lógica de crafting
reglas de inventario
reglas de spoilage
```

---

## Regla principal

El Host ensambla la aplicación.

No es el lugar donde vive el negocio.
