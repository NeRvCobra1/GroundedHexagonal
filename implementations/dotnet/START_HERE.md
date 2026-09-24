# START HERE — ruta de aprendizaje

Esta es la entrada recomendada si Arquitectura Hexagonal todavía se siente como una colección de términos: **Domain, Application, ports, adapters, handlers, repositories, composition roots**.

No intentes entender todo el repositorio en la primera lectura.

El objetivo de esta ruta es ir de una sola idea a la implementación completa.

---

## Antes de empezar: una sola idea

Arquitectura Hexagonal intenta que las reglas importantes del sistema no dependan de cómo entra una petición ni de cómo se guarda la información.

Piensa primero en esta división:

```text
Application coordina
        ↓
Domain decide
```

Todavía no pienses en HTTP, EF Core, SQLite, DI ni Workers.

Lee primero:

[`docs/guides/MinimalExample.md`](docs/guides/MinimalExample.md)

Ahí se usa un ejemplo mínimo para entender esa división antes de abrir `CraftItem`.

---

## Paso 1 — aprende el vocabulario mínimo

Ten abierta:

[`CHEATSHEET.md`](CHEATSHEET.md)

No necesitas memorizarla.

Úsala para traducir cosas como:

```text
Input Port       → operación que el core permite iniciar
Output Port      → capacidad exterior que Application necesita
Inbound Adapter  → traduce una entrada externa al core
Outbound Adapter → implementa una capacidad exterior
Handler          → nombre elegido aquí para una implementación de use case
Repository       → patrón usado por algunos outbound ports
```

Una regla importante:

```text
Port != interface
```

En este proyecto los ports se expresan como interfaces C#, pero ésa es una decisión de implementación.

---

## Paso 2 — mira sólo las capas

Ejecuta el Visualizer:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

Selecciona `CraftItem`.

En la primera vuelta ignora:

```text
nombres de clases
líneas de código
payloads detallados
EF Core
Repository Map
```

Busca solamente:

```text
Inbound Adapter
Input Port
Application
Domain
Output Port
Outbound Adapter
```

La pregunta es:

> ¿Qué responsabilidad tiene cada zona?

Guía completa:

[`docs/guides/Visualizer.md`](docs/guides/Visualizer.md)

---

## Paso 3 — ahora mira Ports y Adapters

Haz otra vuelta por `CraftItem`, pero esta vez identifica fronteras.

```text
HTTP
  ↓
CraftItemEndpoint        Inbound Adapter
  ↓
ICraftItemUseCase        Input Port
  ↓
CraftItemHandler         Application
  ↓
Inventory.Craft          Domain
  ↓
IInventoryRepository     Output Port
  ↓
EfCore/InMemory...       Outbound Adapter
```

Pregunta en cada salto:

> ¿La pieza de arriba conoce una capacidad abstracta o una tecnología concreta?

Application debe poder coordinar el caso de uso sin depender de EF Core.

---

## Paso 4 — sigue un caso de uso real

Ahora sí abre código.

Empieza únicamente con `CraftItem`:

[`docs/guides/GuidedTours.md#tour-1--craftitem`](docs/guides/GuidedTours.md#tour-1--craftitem)

No abras archivos al azar. Sigue el orden del tour.

Cuando termines, intenta explicar:

```text
qué traduce HTTP
qué coordina Application
qué regla decide Domain
qué pide un Output Port
qué implementa el Adapter
quién conecta todo
```

---

## Paso 5 — compara, no memorices

Después recorre:

```text
GetInventory
ProcessFoodSpoilage
```

Tours:

[`docs/guides/GuidedTours.md`](docs/guides/GuidedTours.md)

Busca diferencias:

```text
CraftItem
    cambia estado
    termina persistiendo

GetInventory
    sólo consulta
    no ejecuta SaveAsync

ProcessFoodSpoilage
    no entra por HTTP
    usa un Worker
    usa IClock como outbound port
```

La arquitectura base sigue siendo reconocible aunque cambie el trigger o la tecnología.

---

## Paso 6 — separa Hexagonal de .NET

Cuando ya reconozcas los roles, lee:

[`docs/architecture/HexagonalVsDotNet.md`](docs/architecture/HexagonalVsDotNet.md)

Esto evita aprender equivalencias falsas como:

```text
Hexagonal = interfaces
Hexagonal = Repository Pattern
Hexagonal = ASP.NET Controllers
Hexagonal = proyecto por capa
Hexagonal = EF Core detrás de una interface
```

La arquitectura habla de límites, responsabilidades y dirección de dependencias.

.NET es una forma concreta de materializarlos.

---

## Paso 7 — reconoce errores comunes

Lee:

[`docs/guides/AntiPatterns.md`](docs/guides/AntiPatterns.md)

La meta no es memorizar prohibiciones.

La meta es poder detectar preguntas sospechosas como:

> ¿Por qué mi Domain necesita `DbContext`?

> ¿Por qué mi Handler sabe que usamos SQLite?

> ¿Por qué el Endpoint contiene la regla de crafting?

---

## Paso 8 — practica

Cuando puedas explicar `CraftItem` sin mirar el diagrama, ve a:

[`docs/guides/Exercises.md`](docs/guides/Exercises.md)

Empieza por `RepairItem`.

No copies la estructura archivo por archivo.

Primero decide:

```text
qué regla pertenece a Domain
qué coordina Application
qué capacidades exteriores necesita
cómo entra el caso de uso
qué tests protegen cada decisión
```

---

## Después de esta ruta

Para una lectura más profunda:

[`docs/guides/HowToReadThisImplementation.md`](docs/guides/HowToReadThisImplementation.md)

Para el mapa arquitectónico completo:

[`docs/architecture/README.md`](docs/architecture/README.md)

Para decisiones concretas:

[`docs/adr/`](docs/adr/)

Para conectar IDs, C# y tests:

[`docs/traceability/`](docs/traceability/)

---

## Prueba de comprensión

Antes de considerar terminada esta ruta, intenta responder sin mirar código:

> Si mañana reemplazamos HTTP por una CLI, ¿qué debería cambiar?

> Si reemplazamos SQLite por PostgreSQL, ¿qué debería cambiar?

> Si cambia la regla para poder reparar un item, ¿qué capa debería cambiar?

Si puedes distinguir esos tres tipos de cambio, ya tienes el modelo mental que necesitas para estudiar el resto del proyecto.
