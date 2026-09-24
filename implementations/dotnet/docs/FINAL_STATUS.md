# Estado final — implementación .NET

**Release objetivo:** `v1.0.0`

Este documento define qué se considera terminado en la implementación de referencia .NET del laboratorio.

---

## Alcance completado

```text
[x] Domain puro
[x] Application + use cases
[x] Inbound ports
[x] Outbound ports
[x] HTTP inbound adapter
[x] Worker inbound adapter
[x] InMemory outbound persistence
[x] EF Core + SQLite outbound persistence
[x] SystemClock outbound adapter
[x] API Composition Root
[x] Worker Composition Root
[x] Persistencia seleccionable por configuración
[x] EF Core Migrations
[x] Demo data idempotente
[x] SQLite persistente entre reinicios
[x] Domain tests
[x] Application tests
[x] HTTP integration tests
[x] Worker integration tests
[x] Persistence integration tests
[x] Architecture tests
[x] ADRs
[x] Traceability
[x] Visualizer multi-scenario
[x] Physical Repository Map
[x] Final documentation/readme polish
```

---

## Casos de uso cerrados

### UC-CRAFT-001 — CraftItem

```text
Command
HTTP trigger
Domain mutation
Persistence
```

### UC-INVENTORY-001 — GetInventory

```text
Query
HTTP trigger
Read-only
No SaveAsync
```

### UC-SPOILAGE-001 — ProcessFoodSpoilage

```text
Background Command
Worker trigger
IClock outbound capability
Domain mutation
Persistence
```

---

## Estado de tests

La suite actual contiene `77` tests declarados:

```text
Domain                  15
Application             12
HTTP Integration        12
Persistence Integration 14
Worker Integration       4
Architecture            20
                        ──
Total                   77
```

Antes del tag final deben ejecutarse localmente y pasar todos.

---

## Qué queda fuera de v1.0.0

No es deuda necesaria para considerar terminado el laboratorio:

```text
runtime telemetry/tracing real
OpenTelemetry
SignalR para streaming de eventos
PostgreSQL/Neon
Docker/Cloud deployment
CI/CD
Authentication/Authorization
segunda implementación en otro lenguaje
```

Estas son extensiones opcionales.

---

## Limitación conocida del Visualizer

El click-and-drag del runtime canvas no se considera funcionalidad garantizada.

Alternativas disponibles:

```text
scroll
zoom
focus
follow
selección directa de nodos
```

No bloquea ningún objetivo educativo del proyecto.

---

## Release checklist

En la rama de feature:

```text
[ ] dotnet tool restore
[ ] dotnet build
[ ] dotnet test → 77 passed
[ ] EF model sin pending model changes
[ ] Host.Api arranca
[ ] Visualizer arranca
[ ] CraftItem funciona visualmente
[ ] GetInventory funciona visualmente
[ ] ProcessFoodSpoilage funciona visualmente
[ ] Repository Map funciona
[ ] git status revisado
```

Después:

```text
feature/hexagonal-visualizer
        ↓ Pull Request
main
        ↓
v1.0.0
```

---

## Comandos después del merge

```powershell
git checkout main
git pull

git tag -a v1.0.0 -m "Hexagonal Architecture .NET reference implementation"
git push origin v1.0.0
```

---

## Próximo paso educativo recomendado

No agregar más infraestructura a esta implementación.

La comparación de mayor valor sería implementar la misma specification en una tecnología distinta y responder:

```text
¿Qué cambió por el lenguaje/framework?
¿Qué permaneció igual por la arquitectura?
```

Ese contraste es la siguiente evidencia fuerte de que Hexagonal Architecture no depende de .NET.
