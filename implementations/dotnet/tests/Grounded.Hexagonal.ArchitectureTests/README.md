# Grounded.Hexagonal.ArchitectureTests

Este proyecto contiene pruebas automatizadas que protegen las fronteras estructurales de la implementación .NET.

Su objetivo no es comprobar reglas de negocio.

Su objetivo es verificar que la estructura compilada siga coincidiendo con las decisiones arquitectónicas documentadas.

---

# Dos niveles actuales de protección

Actualmente existen dos familias principales de Architecture Tests.

```text
Nivel 1
    ProjectReference boundaries

Nivel 2
    Assembly + technology boundaries
```

Ambos niveles protegen cosas diferentes.

---

# Nivel 1 — ProjectReference boundaries

`ProjectDependencyTests` inspecciona directamente los `.csproj`.

Protege el grafo productivo:

```text
Domain
    → ninguna referencia a otro proyecto productivo

Application
    → Domain

Inbound.Http
    → Application

Inbound.Worker
    → Application

Outbound.Persistence
    → Application
    → Domain

Outbound.Time
    → Application

Host.Api
    → Application
    → Inbound.Http
    → Outbound.Persistence

Host.Worker
    → Application
    → Inbound.Worker
    → Outbound.Persistence
    → Outbound.Time
```

Esto impide violaciones como:

```text
Domain
    ↓
Application
```

o:

```text
Inbound.Http
    ↓
Outbound.Persistence
```

---

# Nivel 2 — Assembly boundaries

`AssemblyDependencyTests` inspecciona las referencias emitidas dentro de los assemblies compilados.

Esto permite comprobar que el Core no empiece a conocer tecnologías externas.

---

## Domain

Domain no puede referenciar assemblies cuyo nombre comience con:

```text
Microsoft.AspNetCore
Microsoft.EntityFrameworkCore
Grounded.Hexagonal.Adapters
Grounded.Hexagonal.Host
```

La intención es:

```text
Domain
    = negocio
    ≠ framework
```

---

## Application

Application mantiene la misma protección:

```text
Microsoft.AspNetCore
Microsoft.EntityFrameworkCore
Adapters
Hosts
```

Application puede conocer Domain.

No debe conocer las implementaciones externas.

---

## HTTP Adapter

El HTTP adapter sí debe conocer ASP.NET Core.

Ese conocimiento es correcto porque precisamente su responsabilidad es adaptar HTTP hacia Application.

```text
HTTP Adapter
    → Microsoft.AspNetCore.*

Application
    ✕ Microsoft.AspNetCore.*
```

---

## Worker Adapter

El Worker adapter puede conocer abstractions de hosting de .NET, pero no ASP.NET Core.

```text
Inbound.Worker
    ✕ Microsoft.AspNetCore.*
```

---

## Time Adapter

El adapter de tiempo debe permanecer independiente de HTTP y EF Core.

```text
Outbound.Time
    ✕ Microsoft.AspNetCore.*
    ✕ Microsoft.EntityFrameworkCore.*
```

---

## Persistence Adapter

El adapter de persistencia no debe adquirir accidentalmente dependencias HTTP:

```text
Persistence
    ✕ Microsoft.AspNetCore.*
```

Ahora sí debe conocer EF Core porque contiene una implementación tecnológica concreta:

```text
Persistence
    → Microsoft.EntityFrameworkCore.*
```

Esto contrasta deliberadamente con:

```text
Domain
Application
    ✕ Microsoft.EntityFrameworkCore.*
```

---

# Nivel 2 — Project technology boundaries

`ProjectTechnologyBoundaryTests` inspecciona también la configuración declarada en los `.csproj`.

---

## Domain

Actualmente debe tener:

```text
0 PackageReference
0 FrameworkReference
```

---

## Application

Actualmente debe tener:

```text
0 PackageReference
0 FrameworkReference
```

Application conserva únicamente su referencia de proyecto hacia Domain.

---

## HTTP Adapter

Debe declarar explícitamente:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

porque es una class library que implementa un adapter utilizando ASP.NET Core.

---

## Persistence Adapter

Debe declarar:

```text
Microsoft.EntityFrameworkCore.Sqlite
```

porque EF Core + SQLite es ahora una tecnología concreta del outbound adapter.

La misma dependencia sigue estando prohibida en Domain y Application.

---

# Por qué comprobar assembly y csproj

Los dos mecanismos observan problemas distintos.

```text
.csproj test
    comprueba intención/configuración declarada

assembly test
    comprueba referencias que realmente terminaron compiladas
```

Usarlos juntos proporciona una señal más fuerte.

---

# Referencias del proyecto ArchitectureTests

El proyecto de Architecture Tests puede tener referencias hacia los proyectos que inspecciona.

Por ejemplo:

```text
ArchitectureTests
    → Domain
    → Application
    → Http Adapter
    → Worker Adapter
    → Persistence Adapter
    → Time Adapter
```

Eso NO modifica el grafo productivo.

`ArchitectureTests` es código de prueba externo al runtime productivo.

Los tests de Nivel 1 siguen inspeccionando exclusivamente los proyectos dentro de:

```text
src/
```

---

# Qué no se comprueba todavía

Todavía no imponemos reglas excesivamente específicas como:

```text
toda interface debe comenzar con I
todo Handler debe terminar en Handler
todo namespace debe tener exactamente cierta estructura
```

Esas reglas podrían confundir una convención C# con Arquitectura Hexagonal.

Sólo agregaremos reglas cuando protejan una decisión arquitectónica real.

---

# Relación con ADRs

Las principales decisiones relacionadas están documentadas en:

```text
ADR-NET-0001
    Project Boundaries

ADR-NET-0002
    Architecture Tests

ADR-NET-0007
    Core Technology Independence

ADR-NET-0009
    Worker Trigger and Clock Adapter

ADR-NET-0010
    EF Core + SQLite Persistence Adapter
```

---

# Regla principal

Los Architecture Tests no inventan la arquitectura.

El orden correcto es:

```text
decisión
   ↓
documentación
   ↓
implementación
   ↓
Architecture Test
```

Si una decisión cambia conscientemente, deben revisarse juntos:

```text
ADR
README
código
tests
```
