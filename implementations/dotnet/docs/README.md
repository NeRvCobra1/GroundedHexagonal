# Documentación específica de la implementación .NET

Esta carpeta documenta cómo la arquitectura y la specification común se materializan en .NET/C#.

No reemplaza la documentación conceptual ni la specification independiente del lenguaje.

Si eres nuevo en el tema, no empieces recorriendo esta carpeta al azar:

[`../START_HERE.md`](../START_HERE.md)

---

## Índice

```text
docs/
├── architecture/
│   ├── README.md
│   └── HexagonalVsDotNet.md
│
├── guides/
│   ├── README.md
│   ├── MinimalExample.md
│   ├── HowToReadThisImplementation.md
│   ├── GuidedTours.md
│   ├── AntiPatterns.md
│   ├── Exercises.md
│   ├── RunAndValidate.md
│   ├── SQLiteDemo.md
│   └── Visualizer.md
│
├── adr/
│   └── ADR-NET-0001 ... ADR-NET-0013
│
├── traceability/
│   ├── CraftItem.md
│   ├── GetInventory.md
│   └── ProcessFoodSpoilage.md
│
└── FINAL_STATUS.md
```

---

## Por dónde empezar

### Soy nuevo

```text
START_HERE
→ MinimalExample
→ CHEATSHEET
→ Visualizer por capas
→ GuidedTours
```

Enlaces:

- [`../START_HERE.md`](../START_HERE.md)
- [`../CHEATSHEET.md`](../CHEATSHEET.md)
- [`guides/MinimalExample.md`](guides/MinimalExample.md)
- [`guides/GuidedTours.md`](guides/GuidedTours.md)

### Ya conozco Arquitectura Hexagonal

Empieza por:

- [`architecture/README.md`](architecture/README.md)
- [`architecture/HexagonalVsDotNet.md`](architecture/HexagonalVsDotNet.md)
- [`traceability/`](traceability/)
- [`adr/`](adr/)

### Quiero ejecutar el proyecto

Empieza por:

- [`guides/RunAndValidate.md`](guides/RunAndValidate.md)
- [`guides/SQLiteDemo.md`](guides/SQLiteDemo.md)
- [`guides/Visualizer.md`](guides/Visualizer.md)

### Quiero practicar o revisar errores comunes

- [`guides/AntiPatterns.md`](guides/AntiPatterns.md)
- [`guides/Exercises.md`](guides/Exercises.md)

---

## architecture/

Explica:

```text
runtime flow
dependency direction
physical repository map
Domain
Application
Ports
Adapters
Composition Roots
Hexagonal vs .NET / ASP.NET Core / EF Core
```

---

## guides/

Contiene procedimientos educativos y operativos.

No define reglas arquitectónicas nuevas.

Los Guided Tours no reemplazan `traceability/`:

```text
Guided Tours
    orden de lectura del flujo

Traceability
    Architecture ID → C# → tests
```

---

## adr/

Architecture Decision Records específicos de esta implementación.

Una decisión pertenece aquí si existe por elecciones de .NET/C#, framework, persistencia o estructura concreta del proyecto.

---

## traceability/

Conecta:

```text
Architecture ID
    ↓
C# implementation
    ↓
project
    ↓
tests
```

---

## Regla de ubicación documental

Si una afirmación seguiría siendo válida al implementar la misma specification en Java/NestJS/etc., probablemente no debe existir **únicamente** dentro de `implementations/dotnet/docs`.

Si existe porque usamos:

```text
C# interfaces
ASP.NET Core
BackgroundService
EF Core
SQLite
Program.cs
xUnit
```

entonces sí es documentación específica de esta implementación.
