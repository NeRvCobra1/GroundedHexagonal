# CHEATSHEET — Arquitectura Hexagonal + .NET

Referencia rápida. No pretende reemplazar `START_HERE.md` ni los documentos de arquitectura.

---

## El mapa mental

```text
Exterior
   ↓
Inbound Adapter
   ↓
Input Port
   ↓
Application
   ↓
Domain
   ↓
Output Port
   ↓
Outbound Adapter
   ↓
Exterior
```

La ejecución puede salir hacia adapters.

Las dependencias de código del core no deben apuntar a esos adapters.

---

## Conceptos

| Concepto | Pregunta que responde | En este proyecto | No significa automáticamente |
|---|---|---|---|
| **Domain** | ¿Cuáles son las reglas e invariantes? | `Inventory`, `Recipe`, `Food` | EF entity, DTO, service de infraestructura |
| **Application** | ¿Cómo se coordina un caso de uso? | `CraftItemHandler`, `GetInventoryHandler` | lógica HTTP o lógica del ORM |
| **Input Port** | ¿Qué operación ofrece el core? | `ICraftItemUseCase` | “cualquier interface” |
| **Output Port** | ¿Qué capacidad externa necesita Application? | `IInventoryRepository`, `IClock` | “una interface por tecnología” |
| **Inbound Adapter** | ¿Cómo se traduce un trigger externo al core? | Minimal API, Worker | sólo Controllers/HTTP |
| **Outbound Adapter** | ¿Cómo se implementa una capacidad externa? | EF Core, InMemory, `SystemClock` | sólo base de datos |
| **Composition Root** | ¿Dónde se conectan abstracciones e implementaciones? | Hosts / `Program.cs` | lógica de negocio |
| **Use Case** | ¿Qué intención del usuario/sistema se ejecuta? | Craft, GetInventory, Spoilage | endpoint |
| **Entity** | ¿Qué objeto tiene identidad y comportamiento de negocio? | `Inventory`, `Recipe`, `Food` | tabla de BD |

---

## Traducción de nombres .NET

```text
I...                 convención C# para interface
Handler              convención elegida para use cases
Repository           patrón usado para persistencia
Endpoint             mecanismo ASP.NET Core
BackgroundService    mecanismo de hosting .NET
DbContext             detalle EF Core
Program.cs            ubicación concreta del Composition Root
DTO / Request         forma de transporte
```

Ninguno de esos nombres, por sí solo, convierte una solución en Hexagonal.

---

## Regla corta para ubicar lógica

Si la respuesta depende de una **regla del negocio**, empieza mirando Domain.

```text
¿Hay materiales suficientes?
¿Un alimento ya debe estar spoiled?
¿Una reparación supera la durabilidad máxima?
```

Si la pregunta es **qué pasos coordinar para completar una intención**, mira Application.

```text
cargar receta
cargar inventario
pedir al Domain que ejecute la regla
guardar resultado
```

Si la pregunta es **cómo hablar con una tecnología externa**, mira un Adapter.

```text
HTTP → Command
EF Core → SQLite
System clock → DateTimeOffset
```

Si la pregunta es **qué implementación concreta usar**, mira el Composition Root.

---

## Runtime vs dependencias

Runtime:

```text
Application
   ↓ llama
IInventoryRepository
   ↓ implementación ejecutada
EfCoreInventoryRepository
```

Compile-time:

```text
Application
   owns
IInventoryRepository
   ↑ implements
EfCoreInventoryRepository
```

No confundas “a quién se llama” con “de quién depende el proyecto”.

---

## Command vs Query

```text
CraftItem
    Command
    muta Domain
    guarda

GetInventory
    Query
    lee Domain
    no guarda

ProcessFoodSpoilage
    Background Command
    muta Domain
    trigger = Worker
```

Command/Query no sustituyen Ports/Adapters. Son otra dimensión.

---

## Frases que conviene cuestionar

```text
"Todo servicio debe tener interface."
"Todo repository es automáticamente un port."
"Domain debe conocer EF para poder persistirse."
"El Controller es la Application layer."
"Un inbound adapter siempre es HTTP."
"Un outbound adapter siempre es una base de datos."
"Si hay DI ya tenemos inversión de dependencias."
```

---

## Dónde mirar en este repo

```text
src/Grounded.Hexagonal.Domain
    reglas

src/Grounded.Hexagonal.Application
    use cases + ports

src/Grounded.Hexagonal.Adapters.Inbound.*
    entradas

src/Grounded.Hexagonal.Adapters.Outbound.*
    implementaciones externas

src/Grounded.Hexagonal.Host.*
    composición

tests/Grounded.Hexagonal.ArchitectureTests
    fronteras automáticas
```

Para estudiar paso a paso:

[`START_HERE.md`](START_HERE.md)
