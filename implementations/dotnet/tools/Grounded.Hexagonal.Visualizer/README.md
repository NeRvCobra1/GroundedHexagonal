# Grounded.Hexagonal.Visualizer

Herramienta educativa interactiva para observar la implementación .NET del laboratorio de Arquitectura Hexagonal.

> El Visualizer **no forma parte del código productivo**. Vive en `tools/`, no tiene `ProjectReference` hacia los proyectos de `src/` y lee el repositorio en modo read-only.

---

## Ejecutar

Desde `implementations/dotnet`:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

O desde esta carpeta:

```powershell
dotnet run --launch-profile http
```

---

## Qué puede visualizar

Escenarios disponibles:

```text
UC-CRAFT-001      CraftItem
UC-INVENTORY-001  GetInventory
UC-SPOILAGE-001   ProcessFoodSpoilage
```

Cada escenario muestra:

```text
trigger
runtime flow
rol hexagonal de cada paso
Architecture ID
implementación C# concreta
payload actual
source file
snippet con líneas reales
dependencia de proyectos
ubicación física en el repositorio
```

---

## Uso progresivo recomendado

Para no entrar de golpe a todos los detalles, úsalo en cuatro pases:

```text
1. capas y roles
2. Ports / Adapters
3. componentes y casos de uso
4. clases, archivos y snippets
```

La guía detallada está en:

[`../../docs/guides/Visualizer.md`](../../docs/guides/Visualizer.md)

Si eres nuevo en Arquitectura Hexagonal, empieza antes por:

[`../../START_HERE.md`](../../START_HERE.md)

---

## Execution Flow

Reproduce una secuencia educativa del caso de uso.

Ejemplo conceptual:

```text
HTTP
→ Inbound Adapter
→ Input Port
→ Application
→ Domain
→ Output Port
→ Outbound Adapter
→ Exterior
```

El paquete visual cambia para ayudar a seguir transformaciones como:

```text
HTTP JSON
→ DTO
→ Command / Query
→ Domain state
→ persistence representation
→ Application Result
→ HTTP Response
```

El playback es metadata curada. **No es runtime telemetry.**

---

## Repository Map

Escanea `implementations/dotnet` y construye un mapa físico navegable:

```text
área
→ proyecto
→ carpeta
→ archivo
→ símbolo C#
```

Cubre:

```text
src
tests
docs
tools
root/config files
```

Excluye:

```text
bin
obj
.git
.vs
node_modules
```

Cuando el paso activo tiene un archivo C# asociado, Repository Map puede seguir ese archivo y, cuando el scanner lo detecta, su clase/interface/método/propiedad.

---

## Source snippets

Los escenarios almacenan una ruta y un ancla de texto, no una copia del código.

Ejemplo:

```json
{
  "filePath": "src/Grounded.Hexagonal.Application/Ports/Inbound/ICraftItemUseCase.cs",
  "startContains": "public interface ICraftItemUseCase",
  "takeLines": 7
}
```

`SourceSnippetService` abre el archivo real y calcula sus líneas actuales.

---

## Tres mapas, tres preguntas

### Runtime flow

```text
¿Qué se ejecuta después de qué?
```

### Compile-time dependency map

```text
¿Qué proyecto depende de cuál?
```

### Repository map

```text
¿Dónde vive físicamente el código?
```

Separarlas es parte del objetivo educativo.

---

## Controles

La interfaz ofrece:

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

Los nodos `START` y `END` identifican los extremos del flujo.

El canvas también permite **click + drag** para desplazarse por el mapa, además de scroll, Zoom, Focus, Follow y selección directa de nodos.

---

## Limitaciones conocidas

### No es tracing real

El Visualizer no instrumenta `src/` ni escucha una request real.

Una futura herramienta de observabilidad podría usar `ActivitySource`, OpenTelemetry o eventos, pero queda fuera del alcance actual.

### Scanner C# ligero

El Repository Map utiliza detección educativa de símbolos y no reemplaza Roslyn.

### Snapshot del repositorio

El mapa físico se cachea durante la vida del proceso. Reinicia el Visualizer después de cambios estructurales en archivos/carpetas.

---

## Por qué no referencia los proyectos productivos

La herramienta que explica la arquitectura no debe modificar artificialmente la arquitectura que intenta explicar.

Por eso:

```text
src/    sistema observado
tools/  observador educativo
```

El Visualizer puede leer source files, pero no forma parte del grafo productivo.

---

## Documentación relacionada

- [`../../START_HERE.md`](../../START_HERE.md)
- [`../../docs/guides/Visualizer.md`](../../docs/guides/Visualizer.md)
- [`../../docs/architecture/README.md`](../../docs/architecture/README.md)
- [`../../docs/guides/GuidedTours.md`](../../docs/guides/GuidedTours.md)
