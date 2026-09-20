# Grounded.Hexagonal.Adapters.Inbound.Worker

Este proyecto contiene inbound adapters iniciados mediante procesos automáticos, temporizados o ejecutados en background.

Su existencia demuestra que una entrada al sistema no necesita comenzar mediante HTTP.

---

## Implementación actual

```text
FoodSpoilage/
    FoodSpoilageWorker
    FoodSpoilageWorkerOptions
```

Caso de uso:

```text
UC-SPOILAGE-001
ProcessFoodSpoilage
```

---

## Flujo

```text
BackgroundService / Timer
          ↓
FoodSpoilageWorker
          ↓
PORT-IN-SPOILAGE-001
          ↓
Application
```

---

## Responsabilidad

Puede contener:

```text
scheduling
timers
BackgroundService
configuración del trigger
logging del ciclo
traducción hacia inbound ports
```

No debe contener:

```text
reglas de deterioro
persistencia directa
DbContext
SQL
reglas de dominio
```

---

## Dependencias

Puede depender de:

```text
Application
Microsoft.Extensions.Hosting abstractions
```

No debe depender de:

```text
Outbound adapters
Hosts
ASP.NET Core
```

---

## Diferencia frente a HTTP

```text
CraftItem / GetInventory
    HTTP inicia la interacción

ProcessFoodSpoilage
    un scheduler inicia la interacción
```

En ambos casos el adapter termina invocando un inbound port de Application.

---

## Regla principal

`BackgroundService` es tecnología .NET.

`Inbound Adapter` es el rol arquitectónico.
