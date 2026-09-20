# Food — Domain

Esta carpeta contiene el modelo de dominio utilizado por `ProcessFoodSpoilage`.

---

## Food

`Food` representa una instancia individual de alimento perecedero.

Contiene:

```text
FoodId
SpoilsAt
FoodSpoilageState
```

No conoce:

```text
DateTimeOffset.UtcNow
BackgroundService
scheduler
database
repository
HTTP
```

El tiempo actual se entrega explícitamente al comportamiento:

```text
Food.AdvanceSpoilage(currentTime)
```

Esto mantiene la regla determinista y fácil de probar.

---

## RULE-SPOILAGE-001

La primera regla del laboratorio es deliberadamente pequeña:

```text
Fresh + currentTime < SpoilsAt
    → sigue Fresh

Fresh + currentTime >= SpoilsAt
    → cambia a Spoiled

Spoiled
    → permanece Spoiled
```

`AdvanceSpoilage` devuelve `true` únicamente cuando ocurrió una transición real.

---

## Por qué el Domain no consulta el reloj

Esto sería una fuga tecnológica:

```text
Food
    ↓
DateTimeOffset.UtcNow
```

En cambio:

```text
Application obtiene la hora
        ↓
Domain recibe currentTime
```

Domain sólo necesita comprender el concepto de tiempo, no cómo se obtiene.

---

## Regla principal

Domain decide **cuándo un alimento está echado a perder**.

No decide **quién ejecuta periódicamente la revisión** ni **de dónde viene la hora actual**.

---

## Rehidratación

`Food.Restore(...)` permite reconstruir una Entity desde estado que ya fue validado y persistido.

Por ejemplo:

```text
Food persistido:
    State = Spoiled
```

debe volver al Domain como:

```text
Food.State = Spoiled
```

sin volver a ejecutar artificialmente `AdvanceSpoilage`.

Esta factory no conoce EF Core ni SQLite.

Sólo expresa que el Domain puede ser reconstruido desde un estado válido existente.

