# Tests de la implementación .NET

Esta carpeta contiene las pruebas automatizadas de la implementación .NET.

Cada proyecto de tests protege un tipo distinto de responsabilidad.

---

## Proyectos

```text
Grounded.Hexagonal.Domain.Tests
    Reglas puras del dominio.

Grounded.Hexagonal.Application.Tests
    Casos de uso y coordinación de Application.

Grounded.Hexagonal.Http.IntegrationTests
    Integración HTTP.

Grounded.Hexagonal.Worker.IntegrationTests
    Integración del flujo Worker.

Grounded.Hexagonal.Persistence.IntegrationTests
    Integración de persistencia.

Grounded.Hexagonal.ArchitectureTests
    Reglas estructurales y dependencias arquitectónicas.
```

---

## Domain Tests

Deben ejecutarse sin infraestructura externa.

Prueban:

```text
invariantes
entities
value objects
domain rules
domain errors
```

---

## Application Tests

Prueban:

```text
handlers
commands
queries
coordinación de ports
interacción con Domain
```

Normalmente utilizarán implementaciones fake o test doubles de outbound ports.

---

## Integration Tests

Los integration tests verifican interacción entre componentes reales cuando sea necesario.

No deben sustituir las pruebas unitarias del núcleo.

---

## Architecture Tests

Verifican automáticamente reglas como:

```text
Domain no depende de Application.
Domain no depende de Adapters.
Application no depende de Adapters.
Inbound Adapters no dependen de Outbound Adapters.
Outbound Adapters no dependen de Inbound Adapters.
```

Su objetivo es evitar que la estructura arquitectónica se degrade silenciosamente.

---

## Regla principal

Los tests deben demostrar tanto:

```text
que el sistema se comporta correctamente
```

como:

```text
que sigue respetando sus fronteras arquitectónicas
```


---

## Worker Integration Tests

`Grounded.Hexagonal.Worker.IntegrationTests` ahora verifica:

```text
FoodSpoilageWorker
    ↓
IProcessFoodSpoilageUseCase
    ↓
ProcessFoodSpoilageHandler
    ↓
Domain + InMemory persistence
```

También comprueba que el `BackgroundService` inicia automáticamente el inbound port cuando está configurado con `RunImmediately = true`.

Esto prueba el tercer mecanismo de entrada del laboratorio sin introducir HTTP.

---

## SQLite Persistence Integration Tests

`Grounded.Hexagonal.Persistence.IntegrationTests` prueba ahora dos implementaciones concretas de persistencia:

```text
InMemory
EntityFrameworkCore + SQLite
```

Las pruebas SQLite crean una base temporal real por test y verifican:

```text
round-trip de Inventory
round-trip de Recipe
rehidratación de Food
CraftItem
GetInventory
ProcessFoodSpoilage
```

Esto demuestra que los mismos casos de uso funcionan con otro outbound adapter sin modificar Application ni Domain.

