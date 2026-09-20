# FoodSpoilage — Worker Inbound Adapter

```text
Use Case:
UC-SPOILAGE-001

Inbound Port:
PORT-IN-SPOILAGE-001
```

Esta carpeta contiene el adapter que inicia automáticamente el caso de uso `ProcessFoodSpoilage`.

---

## Tecnología concreta

La implementación .NET utiliza:

```text
BackgroundService
```

Esto NO significa:

```text
Inbound Adapter = BackgroundService
```

`BackgroundService` es sólo una de muchas tecnologías posibles para iniciar un inbound port.

---

## Flujo

```text
timer de BackgroundService
        ↓
FoodSpoilageWorker
        ↓
IProcessFoodSpoilageUseCase
        ↓
ProcessFoodSpoilageHandler
```

El worker no conoce:

```text
Food
RULE-SPOILAGE-001
IFoodRepository concreto
SystemClock
SQL
```

---

## Configuración

```text
FoodSpoilage:Interval
FoodSpoilage:RunImmediately
```

pertenecen al mecanismo de scheduling.

No son reglas de negocio.

---

## RunOnceAsync

`RunOnceAsync` hace visible una sola activación del adapter.

Esto facilita pruebas de integración sin esperar realmente un intervalo de producción.

---

## Regla principal

El Worker decide **cuándo pedir que se ejecute el caso de uso**.

Domain decide **qué significa que un alimento se haya echado a perder**.
