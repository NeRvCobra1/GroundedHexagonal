# Documentación específica de la implementación .NET

Esta carpeta contiene documentación relacionada exclusivamente con decisiones, diagramas y trazabilidad de la implementación .NET/C#.

No reemplaza la documentación conceptual ni la especificación independiente del lenguaje.

---

## Estructura

```text
docs/
├── adr/
├── architecture/
├── diagrams/
└── traceability/
```

---

## adr/

Contiene Architecture Decision Records específicos de .NET.

Ejemplos:

```text
separación por assemblies
interfaces para ports
Composition Roots
BackgroundService
persistencia concreta
```

---

## architecture/

Explica cómo los conceptos definidos por la arquitectura general se representan dentro de la solución .NET.

---

## diagrams/

Contiene diagramas de Nivel 3:

```text
clases
interfaces
handlers
adapters
hosts
dependencias entre assemblies
```

Los diagramas conceptuales independientes del lenguaje no deben vivir exclusivamente aquí.

---

## traceability/

Relaciona:

```text
Architecture ID
    ↓
C# type
    ↓
project
    ↓
test
```

---

## Regla principal

Si una decisión sólo existe porque estamos usando .NET/C#, puede documentarse aquí.

Si la decisión seguiría siendo válida en Java, NestJS u otra implementación, probablemente pertenece a la documentación o especificación común.


---

## Último caso documentado

`ProcessFoodSpoilage` agrega:

```text
docs/adr/ADR-NET-0009-worker-trigger-and-clock-adapter.md
docs/traceability/ProcessFoodSpoilage.md
```

La documentación distingue explícitamente el rol arquitectónico de un inbound/outbound adapter de las tecnologías concretas `BackgroundService` y `SystemClock`.

---

## Persistencia relacional

El adapter EF Core + SQLite se documenta mediante:

```text
docs/adr/ADR-NET-0010-ef-core-sqlite-persistence-adapter.md
```

y se refleja en la trazabilidad de los tres casos de uso existentes.

La implementación mantiene separados:

```text
Domain model
Persistence model
```

mediante mapeo explícito.

