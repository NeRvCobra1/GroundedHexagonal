# Ejecutar y validar el laboratorio .NET

Todos los comandos parten desde:

```text
implementations/dotnet
```

---

## Requisitos

El repositorio fija:

```text
.NET SDK 10.0.301
```

mediante `global.json`.

La herramienta local de EF Core está fijada en:

```text
dotnet-ef 10.0.12
```

---

## Validación completa

```powershell
dotnet clean
dotnet tool restore
dotnet restore
dotnet build
dotnet test
```

Estado esperado:

```text
Domain.Tests                  15
Application.Tests             12
Http.IntegrationTests         12
Persistence.IntegrationTests  14
Worker.IntegrationTests        4
ArchitectureTests             20
                              ──
TOTAL                         77
```

---

## Verificar EF Core Migrations

```powershell
dotnet ef migrations list `
  --project src\Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src\Grounded.Hexagonal.Adapters.Outbound.Persistence
```

Debe existir al menos:

```text
20260920220000_InitialCreate
```

Comprobar que el modelo actual coincide con el snapshot:

```powershell
dotnet ef migrations has-pending-model-changes `
  --project src\Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src\Grounded.Hexagonal.Adapters.Outbound.Persistence
```

No debe reportar cambios pendientes del modelo.

> `(Pending)` en `migrations list` sólo indica que esa migration no se ha aplicado a la base concreta que está consultando la factory de design-time; no significa que el archivo de migration sea inválido.

---

## Ejecutar Host.Api

```powershell
dotnet run --project src\Grounded.Hexagonal.Host.Api --launch-profile http
```

En `Development`:

```text
Persistence.Provider = Sqlite
DemoData.Seed         = true
```

La base local por defecto es:

```text
grounded-hexagonal.db
```

La prueba manual completa está en:

[`SQLiteDemo.md`](SQLiteDemo.md)

---

## Ejecutar Host.Worker

```powershell
dotnet run --project src\Grounded.Hexagonal.Host.Worker
```

Este host ejecuta el flujo de spoilage mediante un inbound adapter de background.

---

## Ejecutar Visualizer

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

Validación mínima:

```text
CraftItem                abre y avanza
GetInventory             abre y avanza
ProcessFoodSpoilage      abre y avanza
Repository Map           puede navegar src/tests/docs/tools
Source snippets          muestran archivos reales
Scenario switch          vuelve al paso inicial
```

Más detalle: [`Visualizer.md`](Visualizer.md)

---

## Crear una migration futura

Sólo cuando el modelo EF realmente cambie:

```powershell
dotnet ef migrations add NombreDeLaMigration `
  --project src\Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src\Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --output-dir EntityFrameworkCore\Migrations
```

No ejecutes este comando para “probar” migrations si no hubo un cambio de modelo.

---

## Checklist antes de mergear a main

```text
[ ] git status limpio o cambios entendidos
[ ] dotnet tool restore
[ ] dotnet build
[ ] dotnet test → 77/77
[ ] has-pending-model-changes sin cambios
[ ] Host.Api puede arrancar
[ ] Visualizer puede arrancar
[ ] los tres escenarios del Visualizer funcionan
[ ] documentación relativa no tiene enlaces rotos evidentes
```

---

## Checklist para tag v1.0.0

Después de mergear a `main`:

```powershell
git checkout main
git pull

git status
```

Con `main` limpio y validado:

```powershell
git tag -a v1.0.0 -m "Hexagonal Architecture .NET reference implementation"
git push origin v1.0.0
```

El tag debe marcar el estado estable del laboratorio, no una rama de feature.
