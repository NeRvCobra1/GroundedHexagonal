# Grounded.Hexagonal.Visualizer

Interactive educational visualizer for the .NET reference implementation of the Hexagonal Architecture laboratory.

## Why this project is under `tools/`

The visualizer is **not part of the productive hexagonal implementation**. It observes and explains that implementation.

Therefore it intentionally:

- lives outside `src/`;
- has its own `.csproj` and `.slnx`;
- has no `ProjectReference` to Domain, Application, Hosts or Adapters;
- reads source files as read-only text;
- uses curated scenario metadata instead of instrumenting production code.

This preserves the dependency graph we are trying to study.

## V1 scope

V1 includes `UC-CRAFT-001 / CraftItem` and shows:

- runtime call direction;
- general architectural stage;
- specific Hexagonal Architecture role;
- concrete .NET class/member;
- changing payload representation;
- animated step-by-step playback;
- relevant source lines read from the real `.cs` files;
- the physical project/dependency map;
- the active project/file for the selected runtime step.

The playback is an **educational simulation**, not runtime telemetry.

## Run

From `implementations/dotnet`:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

Or from this folder:

```powershell
dotnet run --launch-profile http
```

The project also explicitly enables static web assets so the Blazor framework script can be served even if the tool is started locally without the launch profile.

Open the URL printed by ASP.NET Core.

## How source snippets work

Scenario metadata stores a relative file path plus a text anchor, for example:

```json
{
  "filePath": "src/Grounded.Hexagonal.Application/Ports/Inbound/ICraftItemUseCase.cs",
  "startContains": "public interface ICraftItemUseCase",
  "takeLines": 7
}
```

At runtime `SourceSnippetService` resolves that file under `implementations/dotnet`, finds the anchor and returns the current line numbers and text.

The visualizer does not copy productive C# into its own source.

## Adding future scenarios

Add a JSON file to `Data/Scenarios/` following `CraftItem.json`.

The intended next scenarios are:

1. `UC-INVENTORY-001 / GetInventory`
2. `UC-SPOILAGE-001 / ProcessFoodSpoilage`

Those scenarios can reuse the same UI and playback engine.

## Important distinction

The upper graph shows **runtime call direction**.

The lower project graph shows **compile-time dependency direction**.

They are intentionally separate because one of the key lessons of Hexagonal Architecture is that runtime calls can travel toward an adapter even while the source-code dependency points back toward a port owned by the core.
