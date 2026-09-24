# Cómo estudiar esta implementación sin perderse

El repositorio está diseñado para aprender por capas. No es necesario leer todos los archivos para comprender la arquitectura.

---

## Ruta recomendada de 30–60 minutos

### 1. Empieza por el Visualizer

Ejecuta:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

Selecciona `CraftItem` y recorre los pasos manualmente.

Busca primero sólo estas etiquetas:

```text
Inbound Adapter
Input Port
Application
Domain
Output Port
Outbound Adapter
```

No abras todavía todos los snippets.

### 2. Abre el Input Port

Localiza:

```text
ICraftItemUseCase
```

Pregunta que responde:

> ¿Qué operación permite ejecutar el sistema?

No preguntes todavía cómo llega HTTP ni cómo se guarda SQLite.

### 3. Abre el Handler

Localiza:

```text
CraftItemHandler
```

Pregunta que responde:

> ¿Cómo coordina Application esta operación?

Observa que usa ports y Domain, no adapters concretos.

### 4. Abre Domain

Localiza:

```text
Inventory.Craft(...)
```

Pregunta que responde:

> ¿Dónde vive la regla que realmente decide si el crafting es válido?

Aquí deben aparecer las invariantes, no en el endpoint ni en EF Core.

### 5. Regresa a los Output Ports

Localiza:

```text
IRecipeRepository
IInventoryRepository
```

Pregunta que responden:

> ¿Qué necesita Application del mundo exterior?

No describen SQLite. Describen capacidades.

### 6. Ahora sí abre el adapter EF Core

Localiza:

```text
EfCoreRecipeRepository
EfCoreInventoryRepository
```

Pregunta que responden:

> ¿Cómo se implementa concretamente esa capacidad en esta tecnología?

### 7. Termina en Host.Api

Localiza:

```text
Program.cs
```

Pregunta que responde:

> ¿Quién conoce tanto el port como el adapter concreto para conectarlos?

Respuesta: el Composition Root.

---

## Después compara los otros dos flujos

### GetInventory

Busca qué **no** ocurre:

```text
no hay Domain mutation
no hay SaveAsync
```

Eso ayuda a distinguir un Query de un Command sin cambiar la arquitectura base.

### ProcessFoodSpoilage

Busca qué cambia en la entrada:

```text
HTTP desaparece
Worker aparece
```

Luego localiza:

```text
IClock
SystemClock
```

Esto demuestra que outbound no significa “database”.

---

## Usa las tres vistas del Visualizer

### Execution Flow

Pregunta:

> ¿Qué se ejecuta después de qué?

### Compile-time Dependency Map

Pregunta:

> ¿Quién puede conocer a quién en código?

### Repository Map

Pregunta:

> ¿Dónde está físicamente esa pieza?

No mezcles estas preguntas. Una de las ideas centrales del laboratorio es que las tres vistas son distintas.

---

## Orden recomendado de carpetas

```text
1. Domain
2. Application
3. Adapters.Inbound.Http
4. Adapters.Outbound.Persistence
5. Host.Api
6. Adapters.Inbound.Worker
7. Adapters.Outbound.Time
8. Host.Worker
9. tests
10. docs/adr
11. docs/traceability
```

---

## Cómo leer los nombres sin confundirlos con la arquitectura

Cuando veas:

```text
ICraftItemUseCase
```

separa mentalmente:

```text
Input Port            concepto arquitectónico
interface + prefijo I representación C#
```

Cuando veas:

```text
CraftItemHandler
```

separa:

```text
Use Case implementation responsabilidad
Handler                 convención de nombres
```

Cuando veas:

```text
EfCoreInventoryRepository
```

separa:

```text
Outbound Adapter  rol arquitectónico
EF Core            tecnología
Repository         patrón utilizado
```

---

## Qué archivos puedes ignorar al principio

En una primera lectura no necesitas estudiar en profundidad:

```text
migrations
launchSettings.json
appsettings
persistence records
CSS/JS del visualizer
assembly plumbing de tests
```

Son importantes, pero no para comprender primero Hexagonal Architecture.

---

## Cuándo leer los ADRs

Lee un ADR cuando aparezca una pregunta de diseño concreta.

Ejemplos:

```text
¿Por qué ports son interfaces?
    → ADR-NET-0003

¿Por qué Application devuelve resultados explícitos?
    → ADR-NET-0004

¿Por qué existe un Worker separado?
    → ADR-NET-0009

¿Por qué Domain no se mapea directamente con EF Core?
    → ADR-NET-0010

¿Por qué los Hosts seleccionan InMemory o SQLite?
    → ADR-NET-0011
```

---

## Cuándo leer la trazabilidad

Cuando quieras pasar de un concepto estable a su implementación concreta:

```text
Architecture ID
    ↓
C# type
    ↓
project
    ↓
test
```

Consulta:

```text
docs/traceability/
```

---

## Pregunta final que deberías poder responder

Después de recorrer el laboratorio, intenta explicar sin mirar código:

> Si mañana cambiamos HTTP por CLI o SQLite por PostgreSQL, ¿qué partes deberían cambiar y cuáles deberían permanecer iguales?

Si puedes separar correctamente adapters, ports, Application y Domain, el objetivo principal del laboratorio está cumplido.
