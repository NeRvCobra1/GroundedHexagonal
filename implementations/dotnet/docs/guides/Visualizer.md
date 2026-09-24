# Grounded Hexagonal Visualizer

El Visualizer es una herramienta educativa separada de la implementación productiva.

Ruta:

```text
tools/Grounded.Hexagonal.Visualizer/
```

No tiene `ProjectReference` hacia Domain, Application, Adapters ni Hosts.

Su propósito es **observar y explicar**, no participar en la ejecución real de los casos de uso.

---

## Ejecutar

Desde `implementations/dotnet`:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

---

## Cómo usarlo sin entrar de golpe a todos los detalles

El Visualizer ya contiene la información necesaria para una lectura progresiva. No hace falta agregar otro modo ni duplicar metadata.

La recomendación es recorrerlo en cuatro pases.

### Pase 1 — capas y roles

Selecciona `CraftItem`.

Ignora snippets, payloads y nombres concretos.

Mira únicamente:

```text
Inbound Adapter
Input Port
Application
Domain
Output Port
Outbound Adapter
```

Pregunta:

> ¿Qué responsabilidad tiene cada etapa?

### Pase 2 — Ports y Adapters

Repite el flujo y ahora observa las transiciones:

```text
CraftItemEndpoint
    ↓
ICraftItemUseCase
    ↓
CraftItemHandler
    ↓
Inventory.Craft
    ↓
IInventoryRepository
    ↓
adapter concreto
```

Pregunta:

> ¿Dónde termina el core y dónde empiezan los mecanismos externos?

### Pase 3 — componentes y casos de uso

Compara los tres escenarios:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

Busca:

```text
qué los inicia
si mutan estado
qué output ports necesitan
qué permanece igual entre HTTP y Worker
```

### Pase 4 — clases, archivos y snippets

Activa la lectura detallada:

```text
source file
real line numbers
code snippet
Active Source Path
Repository Map
```

Ahora ya no estás intentando aprender arquitectura y estructura física al mismo tiempo: estás conectando un modelo mental previo con el C# real.

Esta secuencia corresponde a:

```text
capas
  ↓
Ports / Adapters
  ↓
componentes / casos de uso
  ↓
clases / snippets
```

Ruta completa para principiantes:

[`../../START_HERE.md`](../../START_HERE.md)

---

## Modo 1 — Execution Flow

Permite reproducir:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

Cada paso muestra simultáneamente:

```text
etapa general
rol hexagonal
Architecture ID
implementación C# concreta
payload en ese momento
archivo real
líneas de código relevantes
proyecto activo
```

El playback es una **simulación educativa basada en metadata**.

No es tracing del runtime.

---

## Modo 2 — Repository Map

Escanea el repositorio local en modo read-only y permite navegar:

```text
área
→ proyecto
→ carpeta
→ archivo
→ tipo C#
→ método / propiedad
```

Áreas cubiertas:

```text
src
tests
docs
tools
archivos raíz de dotnet
```

Se excluyen:

```text
bin
obj
.git
.vs
node_modules
```

El mapa puede seguir el archivo asociado al paso activo del Execution Flow.

---

## Las tres vistas que no deben confundirse

### Runtime flow

```text
¿Qué se ejecuta después de qué?
```

### Dependency map

```text
¿Qué proyecto depende de cuál?
```

### Repository map

```text
¿Dónde vive físicamente el código?
```

El objetivo del Visualizer es que esas tres preguntas puedan responderse sin inspeccionar manualmente todo el repositorio.

---

## Snippets de código

Los escenarios no copian el código C#.

Guardan:

```text
ruta relativa
texto ancla
cantidad de líneas
```

`SourceSnippetService` abre el archivo real, localiza el ancla y devuelve las líneas actuales.

Consecuencia:

```text
si el método se desplaza algunas líneas
→ el Visualizer sigue mostrando sus números actuales
```

---

## Repository scanner

El mapa físico utiliza un scanner educativo ligero.

Detecta conceptos como:

```text
class
interface
record
struct
enum
constructor
method
property
```

No pretende reemplazar análisis semántico con Roslyn.

El snapshot del repositorio se mantiene durante la vida del proceso. Si cambias la estructura física del repo, reinicia el Visualizer.

---

## Controles de lectura

La UI incluye controles como:

```text
Play
Pause
Back
Step
Reset
Zoom
Focus
Follow
```

`START` y `END` identifican los extremos del flujo.

La transformación del payload permite ver, por ejemplo:

```text
HTTP JSON
→ Request DTO
→ Command/Query
→ Domain state
→ persistence representation
→ Application Result
→ HTTP Response
```

---

## Navegación del canvas

El canvas permite navegar mediante **click + drag** para desplazarse horizontalmente por el mapa del flujo.

Esta interacción complementa:

```text
scroll del canvas
Zoom
Focus
Follow
selección directa de nodos
```

---

## Por qué está en tools/

Si el Visualizer dependiera directamente de los assemblies productivos sólo para poder explicarlos, alteraría el grafo que estamos intentando estudiar.

Por eso:

```text
src/    implementación observada
tools/  herramienta observadora
```

La separación es deliberada.

---

## Qué sería una V4 opcional

Runtime tracing real podría añadir:

```text
ActivitySource / OpenTelemetry
eventos correlacionados
SignalR / streaming
visualización de una request real
```

Eso continúa fuera del alcance del laboratorio actual, porque el objetivo es estudiar Arquitectura Hexagonal, no observabilidad distribuida.
