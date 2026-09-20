# Grounded.Hexagonal.Adapters.Inbound.Worker

Este proyecto contiene inbound adapters iniciados mediante procesos automáticos, temporizados o ejecutados en background.

Su existencia demuestra que una entrada al sistema no necesita comenzar mediante HTTP.

---

## Responsabilidad arquitectónica

Representa:

```text
Inbound Adapter
```

utilizando mecanismos como:

```text
timer
scheduler
background worker
cron-like execution
```

Ejemplo:

```text
Scheduler
   ↓
Worker Adapter
   ↓
Inbound Port
   ↓
Application
```

---

## Caso principal

El primer caso asociado será:

```text
ProcessFoodSpoilage
```

con Architecture ID:

```text
UC-SPOILAGE-001
```

---

## Puede contener

```text
BackgroundService implementations
timers
scheduling glue code
trigger configuration
mapping hacia inbound ports
```

---

## No debe contener

```text
reglas de deterioro
persistencia directa
SQL
DbContext
reglas de negocio
```

El worker inicia el proceso.

Domain decide las reglas.

Application coordina el caso de uso.

---

## Dependencias

Puede depender de:

```text
Application
```

No debe depender de:

```text
Outbound Persistence Adapter
Hosts
```

---

## Regla principal

`BackgroundService` es una tecnología de .NET.

`Inbound Adapter` es el concepto arquitectónico.

No deben confundirse.
