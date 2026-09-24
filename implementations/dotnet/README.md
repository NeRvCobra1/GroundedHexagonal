# Grounded Hexagonal — implementación de referencia .NET/C#

> Laboratorio educativo de **Arquitectura Hexagonal** en .NET 10. El dominio es ficticio e inspirado en mecánicas de crafting/supervivencia; no pretende reproducir la implementación interna de ningún videojuego real.

**Estado:** candidato a `v1.0.0` · **SDK:** .NET `10.0.301` · **Tests:** `77` · **Visualizer:** incluido en `tools/`

Esta carpeta contiene una implementación completa cuyo objetivo no es sólo “funcionar”, sino hacer visibles las fronteras entre **Domain, Application, Ports, Adapters y Composition Roots**.

---

## Qué demuestra este laboratorio

Los tres casos de uso fueron elegidos para mostrar formas distintas de entrar y salir del hexágono:

| Caso | Tipo | Trigger | Cambia estado | Idea principal |
|---|---|---|---:|---|
| `CraftItem` | Command | HTTP `POST` | Sí | HTTP entra por un inbound adapter; Application coordina; Domain decide; persistencia sale por outbound ports. |
| `GetInventory` | Query | HTTP `GET` | No | Un query puede cruzar las mismas fronteras sin ejecutar `SaveAsync()`. |
| `ProcessFoodSpoilage` | Background Command | Worker | Sí | La entrada no tiene que ser HTTP y un outbound adapter no tiene que ser una base de datos. |

Architecture IDs principales:

```text
UC-CRAFT-001
UC-INVENTORY-001
UC-SPOILAGE-001

PORT-IN-CRAFT-001
PORT-IN-INVENTORY-001
PORT-IN-SPOILAGE-001

PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
PORT-OUT-FOOD-001
PORT-OUT-CLOCK-001
```

---

## Arquitectura en una mirada

### Flujo de ejecución

```text
Actor externo
    │
    ▼
Inbound Adapter
HTTP / Worker
    │
    ▼
Input Port
    │
    ▼
Application Use Case / Handler
    │
    ├──────────────► Domain
    │                 │
    │                 └── reglas e invariantes
    │
    ▼
Output Port
    │
    ▼
Outbound Adapter
EF Core / InMemory / SystemClock
    │
    ▼
Tecnología exterior
SQLite / reloj del sistema
```

### Dirección de dependencias de código

```text
Domain
  ▲
  │
Application
  ▲      ▲
  │      │
Inbound  Outbound
Adapters Adapters
  ▲      ▲
   \    /
    Hosts
```

La dirección de ejecución **no es lo mismo** que la dirección de dependencias. Application puede llamar un outbound port en runtime sin depender del adapter concreto que lo implementa.

---

## Mapa de proyectos

```text
implementations/dotnet/
│
├── src/
│   ├── Grounded.Hexagonal.Domain
│   ├── Grounded.Hexagonal.Application
│   ├── Grounded.Hexagonal.Adapters.Inbound.Http
│   ├── Grounded.Hexagonal.Adapters.Inbound.Worker
│   ├── Grounded.Hexagonal.Adapters.Outbound.Persistence
│   ├── Grounded.Hexagonal.Adapters.Outbound.Time
│   ├── Grounded.Hexagonal.Host.Api
│   └── Grounded.Hexagonal.Host.Worker
│
├── tests/
│   ├── Grounded.Hexagonal.Domain.Tests
│   ├── Grounded.Hexagonal.Application.Tests
│   ├── Grounded.Hexagonal.Http.IntegrationTests
│   ├── Grounded.Hexagonal.Worker.IntegrationTests
│   ├── Grounded.Hexagonal.Persistence.IntegrationTests
│   └── Grounded.Hexagonal.ArchitectureTests
│
├── docs/
│   ├── architecture/
│   ├── guides/
│   ├── adr/
│   └── traceability/
│
└── tools/
    └── Grounded.Hexagonal.Visualizer
```

`tools/Grounded.Hexagonal.Visualizer` está deliberadamente fuera de `src/`: **observa y explica** la implementación, pero no forma parte de la aplicación productiva ni altera su grafo de dependencias.

---

## Inicio rápido

Desde `implementations/dotnet`:

```powershell
dotnet tool restore
dotnet restore
dotnet build
dotnet test
```

Resultado esperado del estado actual:

```text
Grounded.Hexagonal.Domain.Tests                  15
Grounded.Hexagonal.Application.Tests             12
Grounded.Hexagonal.Http.IntegrationTests         12
Grounded.Hexagonal.Persistence.IntegrationTests  14
Grounded.Hexagonal.Worker.IntegrationTests        4
Grounded.Hexagonal.ArchitectureTests             20
                                                ──
TOTAL                                            77
```

La configuración común activa:

```text
Nullable                  enabled
ImplicitUsings            enabled
TreatWarningsAsErrors     true
AnalysisLevel             latest
Deterministic             true
```

---

## Ejecutar la API con SQLite persistente

El perfil `Development` del Host.Api usa SQLite, migrations y demo data idempotente:

```powershell
dotnet run --project src\Grounded.Hexagonal.Host.Api --launch-profile http
```

La guía reproducible está en:

[`docs/guides/SQLiteDemo.md`](docs/guides/SQLiteDemo.md)

Ahí se demuestra:

```text
migrations
→ seed
→ GetInventory
→ CraftItem
→ reinicio
→ estado persistente
```

---

## Ejecutar el Worker

```powershell
dotnet run --project src\Grounded.Hexagonal.Host.Worker
```

Este host demuestra una entrada no HTTP:

```text
BackgroundService
    ↓
FoodSpoilageWorker
    ↓
IProcessFoodSpoilageUseCase
    ↓
ProcessFoodSpoilageHandler
```

---

## Ejecutar el Visualizer

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

El visualizador ofrece dos modos principales:

```text
Execution Flow
    por dónde viaja la información y qué forma tiene en cada paso

Repository Map
    dónde vive físicamente el código: proyecto → carpeta → archivo → símbolo
```

Además separa explícitamente:

```text
runtime call direction
compile-time dependency direction
physical repository location
```

Guía: [`docs/guides/Visualizer.md`](docs/guides/Visualizer.md)

---

## Arquitectura Hexagonal vs convenciones .NET

| Concepto | Rol arquitectónico | Representación en este proyecto |
|---|---|---|
| Inbound Port | Operación que el core expone | Interfaces como `ICraftItemUseCase` |
| Outbound Port | Capacidad externa que Application necesita | `IInventoryRepository`, `IRecipeRepository`, `IFoodRepository`, `IClock` |
| Inbound Adapter | Traduce un trigger externo hacia un input port | Minimal API endpoints, `BackgroundService` |
| Outbound Adapter | Implementa un output port usando tecnología exterior | EF Core repositories, InMemory repositories, `SystemClock` |
| Use Case | Orquesta una operación de aplicación | Clases `...Handler` |
| Composition Root | Conecta ports e implementaciones concretas | `Program.cs` en los Hosts |
| Domain | Reglas e invariantes del negocio | `Inventory`, `Recipe`, `Food`, Value Objects |

Los siguientes nombres **no son requisitos de Arquitectura Hexagonal**:

```text
I...          convención de interfaces en C#
Handler       decisión organizativa
Repository    patrón de acceso a persistencia
Endpoint      mecanismo ASP.NET Core
BackgroundService tecnología de hosting .NET
Program.cs    ubicación concreta del Composition Root
```

La arquitectura define responsabilidades y dependencias; C#/.NET define cómo las materializamos.

---

## Persistencia

La implementación dispone de dos familias de adapters de persistencia:

```text
InMemory
EF Core + SQLite
```

Application sigue dependiendo únicamente de ports.

El Host selecciona el provider mediante configuración:

```text
Persistence:Provider = InMemory | Sqlite
```

SQLite utiliza EF Core Migrations. La CLI está fijada como tool local:

```powershell
dotnet tool restore
```

Comprobación útil:

```powershell
dotnet ef migrations has-pending-model-changes `
  --project src\Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src\Grounded.Hexagonal.Adapters.Outbound.Persistence
```

---

## Tests como protección arquitectónica

El laboratorio prueba dos cosas diferentes:

```text
comportamiento correcto
        +
fronteras arquitectónicas correctas
```

Los Architecture Tests protegen, entre otras, estas reglas:

```text
Domain no depende de Application, Adapters ni Hosts.
Application no depende de Adapters ni Hosts.
Inbound Adapters no dependen de Outbound Adapters.
El core no referencia ASP.NET Core ni EF Core.
El adapter de Time no depende de EF Core.
El Worker inbound adapter no depende del adapter concreto de persistencia.
```

Más detalle: [`tests/README.md`](tests/README.md)

---

## Documentación y trazabilidad

Ruta recomendada:

1. [`docs/architecture/README.md`](docs/architecture/README.md) — mapa conceptual de esta implementación.
2. [`docs/guides/HowToReadThisImplementation.md`](docs/guides/HowToReadThisImplementation.md) — orden recomendado para estudiar el código.
3. [`docs/guides/RunAndValidate.md`](docs/guides/RunAndValidate.md) — comandos de build, tests, migrations y ejecución.
4. [`docs/guides/Visualizer.md`](docs/guides/Visualizer.md) — cómo usar la herramienta visual.
5. [`docs/adr/`](docs/adr/) — decisiones específicas de .NET.
6. [`docs/traceability/`](docs/traceability/) — Specification ID → C# → tests.
7. [`docs/FINAL_STATUS.md`](docs/FINAL_STATUS.md) — alcance cerrado y checklist de release `v1.0.0`.

---

## Decisiones documentadas

Los ADR actuales cubren:

```text
ADR-NET-0001  project boundaries
ADR-NET-0002  architecture tests
ADR-NET-0003  ports as C# interfaces
ADR-NET-0004  explicit application results
ADR-NET-0005  in-memory outbound adapter
ADR-NET-0006  HTTP Minimal API adapter
ADR-NET-0007  core technology independence
ADR-NET-0008  query HTTP adapter
ADR-NET-0009  worker trigger + clock adapter
ADR-NET-0010  EF Core + SQLite persistence adapter
ADR-NET-0011  host-configured persistence composition
ADR-NET-0012  EF Core migrations
ADR-NET-0013  idempotent development demo data
```

---

## Qué no intenta ser este proyecto

No pretende ser:

- una plantilla universal para cualquier sistema;
- una demostración de que “un proyecto por capa” sea obligatorio;
- una réplica de la implementación real de Grounded;
- una guía de observabilidad distribuida;
- una aplicación de producción completa con auth, telemetry, deployment y operaciones cloud.

Es un **laboratorio de arquitectura**, con suficiente infraestructura real para demostrar que las fronteras sobreviven cuando cambian los mecanismos externos.

---

## Estado final del laboratorio .NET

```text
Domain                              ✅
Application                         ✅
Inbound HTTP Adapter                ✅
Inbound Worker Adapter              ✅
InMemory Outbound Adapters          ✅
EF Core + SQLite Adapter            ✅
Time Adapter                        ✅
Composition Roots                   ✅
EF Core Migrations                  ✅
Persistent demo                     ✅
Unit / Integration Tests            ✅
Architecture Tests                  ✅
ADRs + Traceability                 ✅
Interactive Visualizer              ✅
Physical Repository Map             ✅
```

El siguiente paso recomendado no es agregar más capas a esta implementación. Es **cerrar `v1.0.0`** y, si se quiere continuar aprendiendo, implementar la misma specification en otra tecnología para comparar arquitectura vs lenguaje.
