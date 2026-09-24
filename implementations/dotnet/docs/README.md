# Documentación específica de la implementación .NET

Esta carpeta documenta cómo la arquitectura y la specification común se materializan en .NET/C#.

No reemplaza la documentación conceptual ni la specification independiente del lenguaje.

---

## Índice

```text
docs/
├── architecture/
│   └── README.md
│
├── guides/
│   ├── README.md
│   ├── HowToReadThisImplementation.md
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

Si quieres **entender la arquitectura**:

1. [`architecture/README.md`](architecture/README.md)
2. [`guides/HowToReadThisImplementation.md`](guides/HowToReadThisImplementation.md)
3. Visualizer
4. ADRs y traceability según aparezcan dudas concretas

Si quieres **ejecutar el proyecto**:

1. [`guides/RunAndValidate.md`](guides/RunAndValidate.md)
2. [`guides/SQLiteDemo.md`](guides/SQLiteDemo.md)
3. [`guides/Visualizer.md`](guides/Visualizer.md)

Si quieres **cerrar/releasear la implementación**:

1. [`FINAL_STATUS.md`](FINAL_STATUS.md)

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
Hexagonal vs .NET
```

---

## guides/

Contiene procedimientos educativos y operativos.

No define reglas arquitectónicas nuevas.

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
