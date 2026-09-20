# Grounded.Hexagonal.Adapters.Outbound.Time

Este proyecto contiene outbound adapters relacionados con la obtención de tiempo externo al Core.

---

## Implementación actual

```text
PORT-OUT-CLOCK-001
    ↓
IClock
    ↑
SystemClock
```

`SystemClock` utiliza:

```csharp
DateTimeOffset.UtcNow
```

Application no lo hace directamente.

---

## Por qué es un proyecto separado

El reloj no es persistencia.

Colocarlo dentro de:

```text
Adapters.Outbound.Persistence
```

mezclaría dos responsabilidades tecnológicas distintas.

Este proyecto demuestra que un outbound adapter puede representar cualquier capacidad externa:

```text
database
clock
external API
filesystem
message broker
```

no únicamente repositorios.

---

## Dependencias

Puede depender de:

```text
Application
```

No necesita depender de Domain.

No debe depender de:

```text
Inbound Adapters
Hosts
ASP.NET Core
```

---

## Regla principal

Application define que necesita conocer la hora actual.

Este adapter decide obtenerla del reloj del sistema.
