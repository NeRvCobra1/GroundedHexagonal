# Guía manual — SQLite persistente + datos demo

Esta guía demuestra manualmente la diferencia entre:

```text
InMemory
    estado ligado a la vida del proceso

SQLite
    estado persistente en un archivo .db
```

La prueba utiliza los mismos casos de uso:

```text
GetInventory
CraftItem
```

sin modificarlos.

---

## 1. Estado esperado antes de comenzar

Desde:

```text
implementations/dotnet
```

deben pasar:

```powershell
dotnet build
dotnet test
```

El entorno `Development` está configurado para:

```text
Persistence:Provider = Sqlite
DemoData:Seed = true
```

---

## 2. Arrancar la API

Desde `implementations/dotnet`:

```powershell
dotnet run `
  --project src\Grounded.Hexagonal.Host.Api `
  --launch-profile http
```

La API utiliza:

```text
http://localhost:5193
```

Durante el arranque ocurre:

```text
MigrateAsync()
    ↓
aplica InitialCreate si hace falta

EfCoreDemoDataSeeder
    ↓
crea demo data sólo si falta
```

---

## 3. IDs del demo

```powershell
$baseUrl = "http://localhost:5193"

$playerId = "11111111-1111-1111-1111-111111111111"
$recipeId = "22222222-2222-2222-2222-222222222222"

$mintShard = "33333333-3333-3333-3333-333333333333"
$toughGunk = "44444444-4444-4444-4444-444444444444"
$flowerPetal = "55555555-5555-5555-5555-555555555555"
$mintMace = "66666666-6666-6666-6666-666666666666"
```

---

## 4. Consultar inventario inicial

En otra terminal PowerShell:

```powershell
Invoke-RestMethod `
  -Method Get `
  -Uri "$baseUrl/api/inventories/$playerId" |
  ConvertTo-Json -Depth 5
```

El contenido esperado equivale a:

```text
Mint Shard      12
Tough Gunk       5
Flower Petal     8
Mint Mace        0 / ausente
```

La API devuelve IDs, no nombres; la tabla anterior usa los nombres sólo para facilitar la lectura del laboratorio.

---

## 5. Fabricar Mint Mace

```powershell
$body = @{
    playerId = $playerId
    recipeId = $recipeId
} | ConvertTo-Json

Invoke-RestMethod `
  -Method Post `
  -Uri "$baseUrl/api/crafting/items" `
  -ContentType "application/json" `
  -Body $body
```

Resultado esperado:

```json
{
  "status": "Success"
}
```

---

## 6. Consultar nuevamente

```powershell
Invoke-RestMethod `
  -Method Get `
  -Uri "$baseUrl/api/inventories/$playerId" |
  ConvertTo-Json -Depth 5
```

Estado esperado:

```text
Mint Shard       2
Tough Gunk       0 / ausente
Flower Petal     5
Mint Mace        1
```

---

## 7. Probar persistencia entre reinicios

Detén la API con:

```text
Ctrl+C
```

Vuélvela a iniciar:

```powershell
dotnet run `
  --project src\Grounded.Hexagonal.Host.Api `
  --launch-profile http
```

Consulta nuevamente:

```powershell
Invoke-RestMethod `
  -Method Get `
  -Uri "$baseUrl/api/inventories/$playerId" |
  ConvertTo-Json -Depth 5
```

Debe conservar:

```text
Mint Shard       2
Flower Petal     5
Mint Mace        1
```

El seed se ejecutó otra vez, pero detectó que el inventario ya existía y no lo sobrescribió.

---

## 8. ¿Dónde está el archivo SQLite?

La connection string de Development es:

```text
Data Source=grounded-hexagonal.db
```

La ubicación exacta de un path relativo depende del directorio de trabajo del proceso.

Puedes localizarlo desde `implementations/dotnet` con:

```powershell
Get-ChildItem `
  -Path . `
  -Filter grounded-hexagonal.db `
  -Recurse
```

El archivo está ignorado por Git.

---

## 9. Reiniciar completamente el laboratorio

Si quieres volver al inventario inicial, primero detén la API.

Después elimina **únicamente la base local educativa**:

```powershell
Get-ChildItem `
  -Path . `
  -Filter grounded-hexagonal.db `
  -Recurse
```

y elimina el archivo que corresponda al laboratorio.

Al siguiente arranque:

```text
MigrateAsync()
    ↓
crea schema nuevo

SeedIfMissingAsync()
    ↓
crea inventario demo original
```

No utilices esta técnica para una base real con datos importantes.

---

## Qué demuestra arquitectónicamente

Nada de esto requirió modificar:

```text
CraftItemHandler
GetInventoryHandler
Inventory
Recipe
```

La persistencia cambió y el estado ahora sobrevive al proceso, pero el Core continúa igual.

Ese es el punto principal del ejercicio.
