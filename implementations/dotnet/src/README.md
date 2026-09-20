# Código productivo de la implementación .NET

Esta carpeta contiene el código productivo de la implementación de referencia en .NET/C#.

Los proyectos ubicados aquí representan distintas responsabilidades arquitectónicas y tecnológicas.

> La separación física en proyectos `.csproj` es una decisión de esta implementación .NET.
> Arquitectura Hexagonal exige fronteras y direcciones de dependencia claras, pero no exige necesariamente un assembly por responsabilidad.

---

## Relación con la arquitectura

El código productivo se organiza alrededor de cuatro responsabilidades principales:

```text
Core
├── Domain
└── Application

Adapters
├── Inbound
└── Outbound

Hosts
└── Composition Roots
```

En esta implementación se representan mediante los siguientes proyectos:

```text
Grounded.Hexagonal.Domain

Grounded.Hexagonal.Application

Grounded.Hexagonal.Adapters.Inbound.Http

Grounded.Hexagonal.Adapters.Inbound.Worker

Grounded.Hexagonal.Adapters.Outbound.Persistence

Grounded.Hexagonal.Adapters.Outbound.Time

Grounded.Hexagonal.Host.Api

Grounded.Hexagonal.Host.Worker
```

---

# Vista general

```text
src/
│
├── Grounded.Hexagonal.Domain/
│
├── Grounded.Hexagonal.Application/
│
├── Grounded.Hexagonal.Adapters.Inbound.Http/
│
├── Grounded.Hexagonal.Adapters.Inbound.Worker/
│
├── Grounded.Hexagonal.Adapters.Outbound.Persistence/
│
├── Grounded.Hexagonal.Host.Api/
│
└── Grounded.Hexagonal.Host.Worker/
```

Cada proyecto tiene su propio `README.md` con reglas más específicas.

---

# Núcleo de la aplicación

El núcleo está formado por:

```text
Grounded.Hexagonal.Domain
        ▲
        │
Grounded.Hexagonal.Application
```

Estos proyectos contienen el comportamiento que queremos mantener independiente de tecnologías externas.

---

## Grounded.Hexagonal.Domain

Representa el dominio del sistema.

Contiene:

```text
conceptos del negocio
reglas del negocio
entidades
value objects
invariantes
comportamiento de dominio
errores de dominio
```

No debe conocer:

```text
HTTP
ASP.NET Core
Controllers
Endpoints
EF Core
SQL
BackgroundService
Dependency Injection
configuration
```

Su objetivo es expresar el negocio utilizando conceptos propios del dominio.

Ejemplos futuros:

```text
Inventory
Item
Recipe
Food
Crafting
Spoilage
```

---

## Grounded.Hexagonal.Application

Representa la capa de aplicación.

Su responsabilidad principal es:

```text
coordinar casos de uso
```

Application utiliza Domain para ejecutar comportamiento de negocio y define las fronteras necesarias para comunicarse con actores externos.

Aquí encontraremos conceptos como:

```text
Use Cases
Commands
Queries
Handlers
Inbound Ports
Outbound Ports
Application Results
```

Puede depender de:

```text
Domain
```

No debe depender de:

```text
Inbound Adapters
Outbound Adapters
Hosts
ASP.NET Core
EF Core
```

---

# Adapters

Los adapters conectan el núcleo de la aplicación con mecanismos externos.

Se dividen en:

```text
Inbound Adapters
Outbound Adapters
```

Esta distinción describe la dirección de la interacción respecto al núcleo.

---

# Inbound Adapters

Un inbound adapter inicia o traduce una interacción hacia un inbound port de Application.

En esta implementación tenemos inicialmente dos tipos.

---

## Grounded.Hexagonal.Adapters.Inbound.Http

Representa una entrada mediante HTTP.

Su trabajo será traducir conceptos externos como:

```text
HTTP request
route
query string
request body
HTTP headers
HTTP status codes
```

hacia conceptos que Application pueda comprender.

Ejemplo conceptual:

```text
HTTP Request
      ↓
HTTP Adapter
      ↓
Inbound Port
      ↓
Application Use Case
```

Este proyecto puede conocer Application.

No debe contener reglas de negocio.

No debe acceder directamente a la base de datos.

No debe depender del adapter de persistencia.

---

## Grounded.Hexagonal.Adapters.Inbound.Worker

Representa entradas iniciadas mediante procesos automáticos.

Ejemplos:

```text
timer
scheduler
cron-like execution
background worker
```

Ejemplo conceptual:

```text
Timer
  ↓
Worker Adapter
  ↓
Inbound Port
  ↓
Application Use Case
```

Este proyecto existe deliberadamente para demostrar que:

```text
entrada al sistema
```

no significa necesariamente:

```text
HTTP
Controller
Presentation
```

Un inbound adapter puede utilizar cualquier mecanismo capaz de iniciar una interacción con Application.

---

# Outbound Adapters

Los outbound adapters implementan capacidades que Application necesita del exterior.

---

## Grounded.Hexagonal.Adapters.Outbound.Persistence

Representa inicialmente la persistencia del sistema.

Application puede necesitar operaciones como:

```text
obtener inventario
guardar inventario
consultar información persistida
```

pero Application no debe saber:

```text
qué base de datos existe
qué ORM utilizamos
qué tabla almacena la información
cómo se realiza una consulta SQL
```

Application define un outbound port.

El adapter lo implementa.

Ejemplo:

```text
Application
     │
     │ define
     ▼
IInventoryRepository
     ▲
     │ implements
     │
Persistence Adapter
     │
     ▼
Database
```

El proyecto contiene actualmente dos implementaciones:

```text
InMemory
EntityFrameworkCore + SQLite
```

El adapter EF Core conoce detalles tecnológicos como:

```text
DbContext
SQLite
persistence records
explicit mappings
```

sin propagarlos hacia Application o Domain.

---

## Grounded.Hexagonal.Adapters.Outbound.Time

Representa capacidades externas relacionadas con el tiempo.

Actualmente implementa:

```text
PORT-OUT-CLOCK-001
    IClock
        ↓
    SystemClock
```

Este proyecto existe separado de Persistence porque obtener la hora no es una responsabilidad de almacenamiento.

---

# Hosts

Los Hosts son aplicaciones ejecutables.

No representan el dominio ni los casos de uso.

Su responsabilidad principal es:

```text
ensamblar
configurar
iniciar
```

las piezas necesarias para ejecutar el sistema.

---

## Grounded.Hexagonal.Host.Api

Es el Composition Root de la aplicación HTTP.

Puede encargarse de:

```text
arranque de ASP.NET Core
Dependency Injection
configuration
logging
middleware
registrar inbound adapters
registrar outbound adapters
```

Ejemplo:

```text
ICraftItemUseCase
        ↓
CraftItemHandler
```

o:

```text
IInventoryRepository
        ↓
InventoryRepository
```

El Host conoce implementaciones concretas porque necesita conectarlas.

Application no necesita conocer esa composición.

---

## Grounded.Hexagonal.Host.Worker

Es el Composition Root para procesos en background.

Puede encargarse de:

```text
arranque del Worker Host
Dependency Injection
configuration
logging
registro del Worker Adapter
registro de outbound adapters
```

Por ejemplo:

```text
FoodSpoilageWorker
        ↓
IProcessFoodSpoilageUseCase
        ↓
ProcessFoodSpoilageHandler
```

El mecanismo que inicia el caso de uso cambia.

El núcleo no necesita cambiar por ello.

---

# Dirección de dependencias

Las referencias entre proyectos deben mantener la siguiente dirección:

```text
                       Domain
                          ▲
                          │
                     Application
                    ▲           ▲
                    │           │
        Inbound Adapters     Outbound Adapters
                    ▲           ▲
                    │           │
                    └─────┬─────┘
                          │
                        Hosts
```

Una representación más concreta:

```text
Grounded.Hexagonal.Domain
        ▲
        │
Grounded.Hexagonal.Application
        ▲
        │
        ├──────── Grounded.Hexagonal.Adapters.Inbound.Http
        │
        ├──────── Grounded.Hexagonal.Adapters.Inbound.Worker
        │
        ├──────── Grounded.Hexagonal.Adapters.Outbound.Persistence
        └──────── Grounded.Hexagonal.Adapters.Outbound.Time
```

Los Hosts se encuentran en el exterior y pueden conocer los proyectos necesarios para realizar la composición.

---

# Dependencias permitidas

## Domain

Puede depender de:

```text
bibliotecas base de .NET cuando sean necesarias
```

No puede depender de otros proyectos productivos del repositorio.

---

## Application

Puede depender de:

```text
Domain
```

No puede depender de:

```text
Adapters
Hosts
```

---

## Inbound Adapters

Pueden depender de:

```text
Application
```

y, solamente cuando exista una razón explícita, de tipos del Domain expuestos a través de la frontera de Application.

Como regla general se prefiere interactuar con Application.

No pueden depender de:

```text
Outbound Adapters
Hosts
```

---

## Outbound Adapters

Pueden depender de:

```text
Application
Domain
```

No pueden depender de:

```text
Inbound Adapters
Hosts
```

---

### Grounded.Hexagonal.Adapters.Outbound.Time

Representa capacidades externas relacionadas con el tiempo.

Actualmente implementa:

```text
PORT-OUT-CLOCK-001
    IClock
        ↓
    SystemClock
```

Este proyecto existe separado de Persistence porque obtener la hora no es una responsabilidad de almacenamiento.

---

# Hosts

Pueden depender de:

```text
Application
Inbound Adapters
Outbound Adapters
```

porque actúan como Composition Roots.

No deben convertirse en un lugar donde viva lógica de negocio.

---

# Flujo de ejecución y dependencias

Una distinción especialmente importante es que:

```text
dirección del runtime
```

y:

```text
dirección de dependencias
```

no son necesariamente iguales.

Por ejemplo, durante la ejecución podemos tener:

```text
CraftItemEndpoint
       ↓
CraftItemHandler
       ↓
InventoryRepository
       ↓
Database
```

Esto no significa que:

```text
Application
```

dependa de:

```text
Persistence
```

En realidad:

```text
Application
    │
    │ define
    ▼
IInventoryRepository
```

y:

```text
Persistence
    │
    │ implements
    ▼
IInventoryRepository
```

Por ello la dependencia de código apunta hacia Application aunque el flujo de ejecución continúe hacia la infraestructura.

---

# Inversión de dependencias

La implementación utilizará el principio de inversión de dependencias para evitar que el núcleo dependa de detalles externos.

Sin inversión de dependencias:

```text
Application
     ↓
SQL Repository
     ↓
Database
```

Application conocería directamente una implementación tecnológica.

Con inversión:

```text
             Application
                  │
                  ▼
          Outbound Port
                  ▲
                  │
                  │ implements
         Persistence Adapter
                  │
                  ▼
              Database
```

Application controla el contrato.

La infraestructura se adapta a él.

---

# La ubicación física no define por sí sola la arquitectura

Crear carpetas llamadas:

```text
Domain
Application
Infrastructure
```

no garantiza Arquitectura Hexagonal.

La arquitectura depende principalmente de:

```text
responsabilidades
fronteras
dirección de dependencias
independencia del núcleo
```

Por eso esta implementación combina:

```text
documentación
separación por proyectos
Architecture Tests
unit tests
integration tests
```

para proteger esas fronteras.

---

# Organización interna futura

Los proyectos todavía comenzarán relativamente vacíos.

Las carpetas internas se crearán cuando exista una responsabilidad real que representar.

Por ejemplo, Application podrá llegar a contener:

```text
Ports/
    Inbound/
    Outbound/

UseCases/
    CraftItem/
    GetInventory/
    ProcessFoodSpoilage/
```

Pero no se crearán carpetas genéricas simplemente por seguir una plantilla.

Se evitarán estructuras ambiguas como:

```text
Helpers/
Utils/
Managers/
Common/
Misc/
Services/
```

cuando no exista una responsabilidad arquitectónica claramente definida.

Cada carpeta nueva debe poder responder:

> ¿Qué responsabilidad representa y por qué este código pertenece aquí?

---

# Organización por responsabilidad antes que por framework

Siempre que sea posible, el código debe organizarse alrededor del comportamiento y las responsabilidades del sistema.

Por ejemplo:

```text
UseCases/
    CraftItem/
```

es más informativo que una carpeta genérica:

```text
Services/
```

porque permite entender directamente qué comportamiento del sistema representa.

---

# Casos de uso iniciales

Los primeros casos que atravesarán esta estructura serán:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

Cada uno permitirá estudiar un aspecto diferente.

---

## CraftItem

Representará principalmente:

```text
Command
HTTP inbound adapter
Inbound Port
Application Handler
Domain behavior
Outbound Port
Persistence Adapter
```

Flujo aproximado:

```text
HTTP
 ↓
Inbound Adapter
 ↓
CraftItem Port
 ↓
CraftItem Handler
 ↓
Domain
 ↓
Inventory Outbound Port
 ↓
Persistence Adapter
```

---

## GetInventory

Representará principalmente:

```text
Query
HTTP inbound adapter
Inbound Port
Application coordination
Outbound Port
Persistence
```

Permitirá comparar una consulta con un command que modifica estado.

---

## ProcessFoodSpoilage

Representará:

```text
automatic trigger
Worker inbound adapter
Inbound Port
Application Handler
Domain spoilage rules
Persistence
```

Flujo aproximado:

```text
Scheduler / Worker
        ↓
Inbound Worker Adapter
        ↓
ProcessFoodSpoilage Port
        ↓
Application Handler
        ↓
Domain
        ↓
Persistence Port
        ↓
Persistence Adapter
```

Este caso demuestra que la Arquitectura Hexagonal no depende de HTTP.

---

# Trazabilidad

Los proyectos productivos implementarán elementos definidos previamente por la especificación independiente del lenguaje.

Cuando corresponda se utilizarán IDs arquitectónicos como:

```text
UC-CRAFT-001
UC-INVENTORY-001
UC-SPOILAGE-001

PORT-IN-CRAFT-001
PORT-IN-INVENTORY-001
PORT-IN-SPOILAGE-001

PORT-OUT-INVENTORY-001
```

La relación buscada es:

```text
Specification
      ↓
Architecture ID
      ↓
Project
      ↓
C# Type
      ↓
Test
```

La ubicación de un archivo de C# debe poder justificarse mediante esa cadena.

---

# Regla para nuevos proyectos

Antes de agregar un nuevo `.csproj` a `src/` debe poder explicarse:

1. qué responsabilidad arquitectónica o tecnológica representa;
2. por qué esa responsabilidad no pertenece a un proyecto existente;
3. de qué proyectos necesita depender;
4. qué proyectos pueden depender de él;
5. qué frontera ayuda a hacer explícita.

No se deben crear assemblies únicamente para seguir una estructura estética.

---

# Regla para mover código

Mover código entre proyectos no debe considerarse únicamente una reorganización de carpetas.

Puede modificar una frontera arquitectónica.

Antes de mover un tipo debe revisarse:

```text
qué responsabilidad representa
qué dependencias introduce
qué proyecto debería conocerlo
qué Architecture Tests pueden verse afectados
```

---

# Protección de las fronteras

Las reglas descritas en este documento serán protegidas en varios niveles:

```text
README.md
    documentación humana

.csproj references
    dependencias físicas

Architecture Tests
    verificación automática

Unit Tests
    comportamiento interno

Integration Tests
    interacción entre componentes
```

Ningún mecanismo individual reemplaza a los demás.

---

# Principio de lectura

Al navegar por `src/`, la pregunta principal no debe ser:

> ¿Qué framework utiliza esta clase?

La pregunta debe ser:

> ¿Qué responsabilidad cumple esta clase respecto al sistema?

Después puede analizarse cómo .NET/C# implementa esa responsabilidad.

---

# Regla principal de `src/`

El núcleo contiene las decisiones importantes del sistema.

Los adapters traducen entre ese núcleo y el exterior.

Los hosts conectan las implementaciones concretas.

Las dependencias deben dirigirse hacia las abstracciones y reglas más estables, no hacia los detalles tecnológicos.
