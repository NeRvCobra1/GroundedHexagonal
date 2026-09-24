# Arquitectura Hexagonal vs .NET / ASP.NET Core / EF Core

Este documento profundiza una distinción que ya aparece en `architecture/README.md`:

```text
concepto arquitectónico != mecanismo tecnológico
```

El objetivo es evitar que una implementación concreta de .NET se convierta accidentalmente en la definición de Arquitectura Hexagonal.

---

## Dos niveles diferentes

Arquitectura Hexagonal habla de:

```text
responsabilidades
fronteras
dirección de dependencias
cómo entra una intención
cómo el core expresa lo que necesita del exterior
```

.NET / ASP.NET Core / EF Core ofrecen mecanismos para materializar esas decisiones:

```text
interfaces
classes
Dependency Injection
Minimal APIs
BackgroundService
DbContext
Migrations
configuration
```

---

## Correspondencias en este proyecto

| Idea arquitectónica | Representación elegida aquí | ¿Podría cambiar sin dejar de ser Hexagonal? |
|---|---|---:|
| Domain | clases/records C# | Sí |
| Input Port | interface C# | Sí |
| Output Port | interface C# | Sí |
| Use Case | clase `...Handler` | Sí |
| Inbound Adapter | Minimal API / `BackgroundService` | Sí |
| Outbound Adapter | EF Core / InMemory / `SystemClock` | Sí |
| Composition Root | `Program.cs` + composition helpers | Sí |
| Persistencia concreta | EF Core + SQLite | Sí |

Por tanto:

```text
Port != interface
Use Case != Handler por definición
Inbound Adapter != Controller
Outbound Adapter != Repository
Composition Root != Program.cs por definición
```

Son equivalencias de esta implementación, no leyes de la arquitectura.

---

## ASP.NET Core

ASP.NET Core aparece principalmente en los bordes.

Ejemplos:

```text
CraftItemEndpoint
GetInventoryEndpoint
Host.Api
```

Su responsabilidad es resolver mecanismos como:

```text
routing
HTTP request/response
dependency injection
startup
hosting
```

No debería decidir:

```text
si hay ingredientes suficientes
cómo evoluciona Food a Spoiled
qué invariantes protege Inventory
```

Por eso el core no referencia ASP.NET Core.

---

## EF Core

EF Core es un mecanismo de persistencia.

En el proyecto vive fuera de Application y Domain:

```text
Application
    ↓ necesita
IInventoryRepository
    ↑ implementa
EfCoreInventoryRepository
    ↓ usa
EF Core / SQLite
```

Application sabe:

```text
necesito cargar y guardar Inventory
```

No sabe:

```text
uso DbContext
uso SQLite
uso migrations
qué tabla contiene los datos
```

Además, los modelos de persistencia no obligan a Domain a convertirse en modelos del ORM.

---

## Dependency Injection

DI ayuda a realizar el wiring:

```text
IInventoryRepository
        ↓
EfCoreInventoryRepository
```

pero DI no es Arquitectura Hexagonal.

Podrías usar:

```text
wiring manual
otro container
otro runtime
```

y conservar la misma separación conceptual.

Lo importante es quién posee la abstracción y hacia dónde apunta la dependencia.

---

## Interfaces

C# hace natural expresar ports con interfaces.

Este proyecto utiliza:

```text
ICraftItemUseCase
IInventoryRepository
IClock
```

Pero una interface sólo es un port cuando representa una frontera con significado arquitectónico.

Esto:

```csharp
public interface ISqliteHelper
```

no se vuelve automáticamente un port sólo porque tenga `I`.

---

## Repository Pattern

En este proyecto algunos output ports usan semántica de repository:

```text
IInventoryRepository
IRecipeRepository
IFoodRepository
```

Eso es útil porque Application necesita cargar/persistir esos conceptos.

Pero Hexagonal Architecture no exige que todo output port sea repository.

Prueba:

```text
IClock
```

Es un output port y no tiene nada que ver con una base de datos.

---

## BackgroundService

`FoodSpoilageWorker` demuestra:

```text
Inbound Adapter != HTTP
```

`BackgroundService` es la tecnología .NET elegida para producir un trigger periódico.

Podría sustituirse por:

```text
scheduler
message consumer
cron
job runner
```

El core seguiría exponiendo:

```text
IProcessFoodSpoilageUseCase
```

---

## ¿Qué cambia si cambia la tecnología?

### HTTP → CLI

Esperado:

```text
nuevo inbound adapter
nuevo host/composition si aplica
```

Debería sobrevivir:

```text
Application
Domain
output ports
persistence adapters
```

### SQLite → PostgreSQL

Esperado:

```text
persistence adapter/configuración
integration tests específicos
composition
```

Debería sobrevivir:

```text
Domain
Application
input ports
HTTP adapter
Worker adapter
```

### EF Core → otra tecnología de datos

Esperado:

```text
outbound adapter
mapping
persistence integration tests
```

Los casos de uso no deberían reescribirse sólo por cambiar ORM.

---

## Lo que los Architecture Tests protegen

Las convenciones anteriores no se dejan únicamente como texto.

El proyecto comprueba restricciones como:

```text
Domain no depende de Application/Adapters/Hosts
Application no depende de Adapters/Hosts
Core no referencia ASP.NET Core
Core no referencia EF Core
Inbound Adapters no dependen de Outbound Adapters
```

Así se protege una propiedad arquitectónica independientemente de cómo se nombren las carpetas.

---

## Pregunta de comprobación

Toma cualquier tipo del proyecto y pregúntate:

> ¿Su rol existe porque la arquitectura lo necesita o porque .NET ofrece ese mecanismo?

Ejemplos:

```text
Output Port
    arquitectura

IInventoryRepository
    representación C# concreta del port

Inbound Adapter
    arquitectura

Minimal API
    tecnología concreta del adapter
```

Cuando puedes mantener separadas esas dos columnas, resulta mucho más fácil llevar el mismo diseño a Java, TypeScript, Python u otro stack.
