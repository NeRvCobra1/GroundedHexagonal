# Grounded Hexagonal

Laboratorio educativo de **Arquitectura Hexagonal** con una implementación de referencia en **.NET 10 / C#**.

El objetivo del repositorio no es enseñar una receta única de carpetas. Busca hacer visible una idea más importante:

```text
los mecanismos externos pueden cambiar
sin mover las reglas del negocio al framework
```

La implementación incluye Domain, Application, ports, inbound/outbound adapters, API, Worker, persistencia InMemory y EF Core/SQLite, Architecture Tests, un Visualizer interactivo y automatización de CI / Continuous Delivery.

---

## Elige tu puerta de entrada

| Si tú... | Empieza aquí |
|---|---|
| **Soy nuevo en Arquitectura Hexagonal** | [`START_HERE.md`](implementations/dotnet/START_HERE.md) |
| **Quiero una referencia rápida** | [`CHEATSHEET.md`](implementations/dotnet/CHEATSHEET.md) |
| **Ya conozco Hexagonal** | [`docs/architecture/README.md`](implementations/dotnet/docs/architecture/README.md) |
| **Quiero seguir código real paso a paso** | [`GuidedTours.md`](implementations/dotnet/docs/guides/GuidedTours.md) |
| **Quiero explorar visualmente** | [`Visualizer.md`](implementations/dotnet/docs/guides/Visualizer.md) |
| **Quiero entender errores comunes** | [`AntiPatterns.md`](implementations/dotnet/docs/guides/AntiPatterns.md) |
| **Quiero practicar** | [`Exercises.md`](implementations/dotnet/docs/guides/Exercises.md) |
| **Quiero separar arquitectura de .NET/ASP.NET/EF Core** | [`HexagonalVsDotNet.md`](implementations/dotnet/docs/architecture/HexagonalVsDotNet.md) |
| **Quiero ejecutar o validar el proyecto** | [`RunAndValidate.md`](implementations/dotnet/docs/guides/RunAndValidate.md) |

> Si no sabes cuál elegir, empieza por `START_HERE.md`. Está diseñada para evitar entrar de golpe al caso `CraftItem` completo.

---

## Implementación de referencia

La implementación principal vive en:

```text
implementations/dotnet/
```

Los tres casos de uso del laboratorio son:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

Sirven para comparar:

```text
Command con mutación
Query de sólo lectura
Background Command disparado por Worker
```

README técnico completo:

[`implementations/dotnet/README.md`](implementations/dotnet/README.md)

---

## Idea central

Durante la ejecución puedes ver algo como:

```text
HTTP
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
```

pero la dirección de dependencias de código es diferente:

```text
Domain
  ▲
Application
  ▲
Adapters
  ▲
Hosts
```

Ese contraste entre **runtime flow** y **compile-time dependency direction** es una de las ideas principales del laboratorio.

---

## Estado

La implementación .NET de referencia cuenta con:

```text
77 automated tests
Architecture Tests
EF Core + SQLite
InMemory adapters
HTTP + Worker inbound adapters
Interactive Visualizer
Repository Map
CI with GitHub Actions
Continuous Delivery with versioned GitHub Releases
```

Continuous Deployment a un proveedor cloud no forma parte del estado final del laboratorio.

---

## DevOps

La automatización de GitHub Actions se documenta brevemente en:

[`README_DEVOPS.md`](README_DEVOPS.md)

El flujo actual diferencia:

```text
Continuous Integration
    PR → build → tests → architecture validation

Continuous Delivery
    version tag → publish → package → GitHub Release
```
