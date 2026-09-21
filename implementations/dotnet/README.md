# Implementación de referencia .NET/C#

Esta carpeta contiene la implementación de referencia en .NET/C# de la especificación común del proyecto Grounded Hexagonal.

> Esta implementación no define la Arquitectura Hexagonal ni la especificación funcional del sistema. Traduce ambos conceptos a decisiones concretas de .NET y C#.

---

## Relación con el resto del repositorio

El proyecto distingue deliberadamente tres niveles:

```text
docs/
    Explica los conceptos de Arquitectura Hexagonal.

specification/
    Define qué debe hacer el sistema de manera independiente del lenguaje.

implementations/dotnet/
    Implementa esa especificación utilizando .NET y C#.
```

Una decisión encontrada dentro de esta carpeta no debe asumirse automáticamente como una regla de Arquitectura Hexagonal.

Por ejemplo:

* un inbound port es un concepto arquitectónico;
* representarlo mediante una `interface` es una decisión de C#;
* ejecutar un inbound adapter mediante ASP.NET Core es una decisión tecnológica;
* ejecutar otro mediante `BackgroundService` es otra decisión tecnológica.

---

## Objetivos

Esta implementación tiene dos objetivos simultáneos:

1. proporcionar una aplicación funcional, compilable y probada;
2. servir como material educativo para estudiar cómo una Arquitectura Hexagonal puede implementarse en .NET.

Por ese motivo se priorizan fronteras arquitectónicas explícitas, trazabilidad y documentación por encima de reducir artificialmente el número de proyectos o archivos.

---

## Estructura

```text
src/
    Código productivo.

tests/
    Unit tests, integration tests y architecture tests.

docs/
    Documentación específica de la implementación .NET.
```

La especificación independiente del lenguaje se encuentra fuera de esta carpeta, en:

```text
/specification/
```

---

## Proyectos productivos

```text
Grounded.Hexagonal.Domain
    Modelo y reglas puras del dominio.

Grounded.Hexagonal.Application
    Casos de uso y ports utilizados para comunicarse con el exterior.

Grounded.Hexagonal.Adapters.Inbound.Http
    Traduce interacciones HTTP hacia inbound ports.

Grounded.Hexagonal.Adapters.Inbound.Worker
    Inicia casos de uso desde procesos automáticos o temporizados.

Grounded.Hexagonal.Adapters.Outbound.Persistence
    Implementa ports relacionados con persistencia.
    Incluye adapters In-Memory y EF Core + SQLite.

Grounded.Hexagonal.Adapters.Outbound.Time
    Implementa ports relacionados con capacidades temporales, como IClock.

Grounded.Hexagonal.Host.Api
    Composition Root de la aplicación HTTP.

Grounded.Hexagonal.Host.Worker
    Composition Root de los procesos en background.
```

---

## Dirección de dependencias

La dirección de ejecución y la dirección de dependencias no son necesariamente iguales.

Las dependencias de código deben apuntar hacia el núcleo de la aplicación.

```text
                      Domain
                         ▲
                         │
                    Application
                    ▲           ▲
                    │           │
           Inbound Adapters   Outbound Adapters
                    ▲           ▲
                    └─────┬─────┘
                          │
                        Hosts
```

Reglas fundamentales:

```text
Domain
    no depende de Application, Adapters ni Hosts.

Application
    puede depender de Domain.
    no depende de Adapters ni Hosts.

Inbound Adapters
    pueden depender de Application.
    no dependen de Outbound Adapters.

Outbound Adapters
    pueden depender de Application y Domain.
    no dependen de Inbound Adapters.

Hosts
    pueden conocer las implementaciones concretas necesarias
    para realizar la composición y Dependency Injection.
```

Estas reglas serán verificadas automáticamente mediante Architecture Tests.

---

## Flujo de ejecución vs. dirección de dependencias

En Arquitectura Hexagonal es importante distinguir cómo se ejecuta el sistema de cómo están organizadas sus dependencias.

Por ejemplo, en tiempo de ejecución podemos tener:

```text
HTTP Request
    ↓
HTTP Adapter
    ↓
Application
    ↓
Persistence Adapter
    ↓
Database
```

Sin embargo, Application no depende directamente del Persistence Adapter.

Application define el contrato que necesita:

```text
Application
    │
    └── Outbound Port
```

y el adapter externo implementa ese contrato:

```text
Persistence Adapter
        │
        └── implements
                ↓
        Application Port
```

Esto permite mantener el núcleo independiente de tecnologías externas.

---

## Casos de uso de referencia

La implementación desarrollará inicialmente tres casos principales:

```text
CraftItem
    Command iniciado desde un inbound adapter HTTP.

GetInventory
    Query iniciada desde un inbound adapter HTTP.

ProcessFoodSpoilage
    Proceso iniciado por un inbound adapter Worker/Scheduler.
```

`ProcessFoodSpoilage` existe deliberadamente para demostrar que un caso de uso no necesita comenzar mediante HTTP, un Controller o una interfaz gráfica.

Una acción en Arquitectura Hexagonal puede comenzar desde distintos tipos de inbound adapters, por ejemplo:

```text
HTTP / REST
UI
CLI
Background Worker
Scheduler / Cron
Message Consumer
Event Consumer
Webhook
gRPC
WebSocket
```

Por lo tanto, `Presentation` no debe utilizarse como sinónimo universal de entrada al sistema.

---

## Ports

Los ports representan las fronteras mediante las cuales el núcleo se comunica con el exterior.

Distinguimos dos tipos principales.

### Inbound Ports

Representan operaciones que el sistema permite ejecutar.

Ejemplos:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

En esta implementación .NET probablemente serán representados mediante interfaces de C#, pero esa es una decisión de implementación.

Conceptualmente:

```text
Inbound Port
```

no significa:

```text
C# interface
```

La interface es solamente una posible representación concreta del concepto arquitectónico.

---

### Outbound Ports

Representan capacidades externas que Application necesita para completar un caso de uso.

Por ejemplo:

```text
obtener inventario
guardar inventario
consultar recetas
obtener la hora actual
```

Application define qué necesita.

Los adapters externos deciden cómo hacerlo.

Por ejemplo:

```text
Application
    ↓
IInventoryRepository
    ▲
    │ implements
    │
Persistence Adapter
```

Application no necesita conocer si la persistencia utiliza:

```text
SQL Server
PostgreSQL
SQLite
In-Memory
API externa
archivo
```

---

## Adapters

Los adapters traducen entre el mundo externo y los ports del sistema.

### Inbound Adapters

Inician interacciones con el núcleo.

Ejemplos de esta implementación:

```text
Grounded.Hexagonal.Adapters.Inbound.Http
Grounded.Hexagonal.Adapters.Inbound.Worker
```

El adapter HTTP traducirá conceptos como:

```text
HTTP Request
Route
Body
Status Code
```

hacia conceptos que Application pueda entender.

El Worker traducirá conceptos como:

```text
timer
scheduler
background execution
```

hacia una llamada a un inbound port.

---

### Outbound Adapters

Implementan capacidades que Application necesita del exterior.

Ejemplo:

```text
Grounded.Hexagonal.Adapters.Outbound.Persistence
```

Este proyecto podrá contener detalles como:

```text
EF Core
DbContext
SQL
Persistence Models
Mappings
Repository implementations
```

Estos detalles no deben propagarse hacia Domain o Application.

---

## Hosts y Composition Root

Los proyectos Host son ejecutables responsables de iniciar y ensamblar la aplicación.

Actualmente existen:

```text
Grounded.Hexagonal.Host.Api
Grounded.Hexagonal.Host.Worker
```

Los Hosts son responsables de tareas como:

```text
startup
Dependency Injection
configuration
framework initialization
adapter registration
logging configuration
```

Aquí se realiza la composición entre ports e implementaciones concretas.

Por ejemplo:

```text
IInventoryRepository
        ↓
EfCoreInventoryRepository
```

El Host puede conocer ambos porque necesita conectarlos.

Esto no significa que Application conozca la implementación concreta.

---

## Domain

`Grounded.Hexagonal.Domain` contiene el conocimiento central del negocio.

Aquí vivirán conceptos como:

```text
Item
Inventory
Recipe
Food
Crafting rules
Spoilage rules
Value Objects
Domain Errors
```

Domain no debe conocer detalles tecnológicos como:

```text
HTTP
ASP.NET Core
EF Core
SQL
JSON
Controllers
BackgroundService
Dependency Injection
```

El dominio debe poder entenderse y probarse sin levantar ninguna infraestructura externa.

---

## Application

`Grounded.Hexagonal.Application` coordina los casos de uso del sistema.

Aquí se encontrarán conceptos como:

```text
Use Cases
Handlers
Commands
Queries
Inbound Ports
Outbound Ports
Application Results
```

Application puede utilizar Domain para ejecutar las reglas del negocio.

Application no debe conocer implementaciones concretas de infraestructura.

Por ejemplo, puede depender de:

```text
IInventoryRepository
```

pero no de:

```text
EfCoreInventoryRepository
GroundedDbContext
```

---

## Convenciones de nombres de C#

Algunos nombres utilizados en esta implementación son convenciones o patrones de C#, no conceptos obligatorios de Arquitectura Hexagonal.

### `I...`

Ejemplo:

```text
IInventoryRepository
ICraftItemUseCase
```

La letra `I` indica una interface de C#.

No forma parte de Arquitectura Hexagonal.

---

### Handler

Ejemplo:

```text
CraftItemHandler
```

Un Handler será utilizado para implementar u orquestar un caso de uso.

La Arquitectura Hexagonal no exige que una clase se llame `Handler`.

Es una decisión organizativa de esta implementación.

---

### Repository

Ejemplo:

```text
IInventoryRepository
InventoryRepository
```

Repository es un patrón utilizado para abstraer acceso a persistencia.

Dependiendo de dónde se encuentre:

```text
IInventoryRepository
```

puede representar un outbound port.

Mientras que:

```text
InventoryRepository
```

puede representar el outbound adapter que implementa dicho port.

---

### Controller / Endpoint

Un Controller o Endpoint de ASP.NET Core puede funcionar como inbound adapter.

No representa Application ni Domain.

Tampoco significa que todas las interacciones con el sistema deban iniciar mediante HTTP.

---

## Trazabilidad

Los elementos relevantes utilizarán los IDs definidos por la especificación.

Ejemplos:

```text
UC-CRAFT-001

UC-INVENTORY-001

UC-SPOILAGE-001

PORT-IN-CRAFT-001

PORT-IN-INVENTORY-001

PORT-IN-SPOILAGE-001

PORT-OUT-INVENTORY-001

RULE-CRAFT-001
```

Cuando un elemento de C# implemente uno de estos conceptos, su documentación debe indicar el ID correspondiente.

Esto permite seguir el recorrido:

```text
Specification
      ↓
Architecture ID
      ↓
.NET Documentation
      ↓
C# Implementation
      ↓
Tests
```

Los nombres concretos de las clases podrán cambiar con el tiempo.

Los IDs arquitectónicos deben mantenerse estables mientras el concepto arquitectónico siga siendo el mismo.

---

## Documentación dentro del código

Las fronteras arquitectónicas relevantes deben documentarse también dentro del código cuando ayude a comprender su responsabilidad.

Por ejemplo, una interface que implemente un inbound port puede indicar:

```text
Architecture ID
Use Case relacionado
Specification relacionada
Responsabilidad arquitectónica
```

La documentación debe explicar principalmente:

```text
por qué existe el elemento
qué responsabilidad tiene
qué concepto arquitectónico representa
```

y no limitarse a describir sintaxis evidente del código.

---

## Architecture Tests

La solución contiene:

```text
Grounded.Hexagonal.ArchitectureTests
```

Este proyecto verificará automáticamente reglas estructurales de la arquitectura.

Por ejemplo:

```text
Domain no depende de Application.

Domain no depende de Adapters.

Domain no depende de Hosts.

Application no depende de Adapters.

Application no depende de Hosts.

Inbound Adapters no dependen de Outbound Adapters.

Outbound Adapters no dependen de Inbound Adapters.
```

El objetivo es que las reglas arquitectónicas no existan únicamente como documentación.

También deben ser verificables automáticamente.

---

## ADRs

Las decisiones arquitectónicas importantes específicas de .NET serán documentadas mediante Architecture Decision Records.

Se encuentran en:

```text
docs/adr/
```

Ejemplos de decisiones que pueden documentarse ahí:

```text
separación por assemblies
ports representados mediante interfaces
uso de Composition Roots
uso de BackgroundService
estrategia de persistencia
```

Las decisiones independientes del lenguaje no deben documentarse exclusivamente aquí.

Deben vivir en la documentación o especificación común del repositorio.

---

## Documentación jerárquica

Esta implementación utiliza documentación jerárquica mediante archivos `README.md`.

La intención es que sea posible abrir cualquier carpeta arquitectónicamente relevante y comprender:

```text
qué representa
qué responsabilidad tiene
qué contiene
qué no debe contener
de qué puede depender
quién puede depender de ella
cómo se relaciona con Arquitectura Hexagonal
cómo se implementa específicamente en C#
```

Los README de niveles inferiores deben complementar, no duplicar completamente, la documentación de niveles superiores.

---

## Tests

Los tests se dividen por responsabilidad.

```text
Grounded.Hexagonal.Domain.Tests
    Pruebas unitarias de reglas de dominio.

Grounded.Hexagonal.Application.Tests
    Pruebas de casos de uso y coordinación de Application.

Grounded.Hexagonal.Http.IntegrationTests
    Pruebas de integración del adapter/host HTTP.

Grounded.Hexagonal.Worker.IntegrationTests
    Pruebas relacionadas con la ejecución mediante Worker.

Grounded.Hexagonal.Persistence.IntegrationTests
    Pruebas de integración de persistencia.

Grounded.Hexagonal.ArchitectureTests
    Pruebas automáticas de reglas arquitectónicas.
```

Cada tipo de test tiene una responsabilidad distinta.

---

## Política de build

La configuración común de los proyectos .NET se encuentra en:

```text
Directory.Build.props
```

Esta configuración pertenece al tooling y calidad de la implementación .NET.

No forma parte de la especificación independiente del lenguaje.

El proyecto busca mantener:

```text
build limpio
warnings tratados como errores
nullable habilitado
análisis estático
builds deterministas
```

---

## Principio de independencia

La intención principal de esta implementación puede resumirse así:

```text
Domain
    no sabe cómo se ejecuta la aplicación.

Application
    no sabe qué tecnologías externas implementan sus necesidades.

Adapters
    conocen tecnologías externas y traducen hacia/desde los ports.

Hosts
    ensamblan las piezas concretas.
```

Por lo tanto:

```text
Business Rules
        │
        │ no conocen
        ▼
HTTP
SQL
ASP.NET Core
EF Core
BackgroundService
```

---

## Regla principal

El código .NET implementa la arquitectura.

El código .NET no define qué significa Arquitectura Hexagonal.

La especificación describe el comportamiento esperado.

La arquitectura define las fronteras y responsabilidades.

.NET/C# proporciona una implementación concreta de ambas.


---

## Estado de ProcessFoodSpoilage

La implementación .NET incluye ahora el primer caso de uso iniciado sin HTTP:

```text
FoodSpoilageWorker
    ↓
PORT-IN-SPOILAGE-001
    ↓
ProcessFoodSpoilageHandler
    ↓
Food.AdvanceSpoilage(...)
```

El tiempo se obtiene mediante:

```text
PORT-OUT-CLOCK-001
    → IClock
    → SystemClock
```

La decisión está documentada en `ADR-NET-0009`.


---

## Selección de adapters en los Hosts

Los ejecutables son los **Composition Roots** y deciden qué adapters concretos utilizar.

La persistencia puede seleccionarse mediante configuración:

```text
Persistence:Provider = InMemory | Sqlite
```

Con SQLite se utiliza:

```text
ConnectionStrings:Grounded
```

Esto permite cambiar:

```text
InMemory repositories
        ↕
EF Core + SQLite repositories
```

sin modificar Domain, Application ni los inbound adapters.

La decisión está documentada en:

```text
docs/adr/ADR-NET-0011-host-configured-persistence-composition.md
```


---

# EF Core Migrations

La implementación SQLite utiliza EF Core Migrations para evolucionar el schema.

La herramienta `dotnet-ef` está fijada como tool local del repositorio:

```powershell
dotnet tool restore
```

El runtime aplica migrations pendientes mediante:

```text
EfCoreDatabaseInitializer
    → MigrateAsync
```

Los detalles y comandos viven en:

```text
src/Grounded.Hexagonal.Adapters.Outbound.Persistence/
    EntityFrameworkCore/
        README.md
```

Esta preocupación permanece fuera de Domain y Application.


---

## Laboratorio manual con SQLite

El entorno `Development` del Host.Api utiliza SQLite y datos demo idempotentes para permitir una prueba manual completa de:

```text
Migrations
    ↓
Seed
    ↓
GetInventory
    ↓
CraftItem
    ↓
reinicio del proceso
    ↓
estado persistente
```

Guía:

```text
docs/guides/SQLiteDemo.md
```

Los tests continúan usando configuración aislada y no dependen del archivo `.db` local.
