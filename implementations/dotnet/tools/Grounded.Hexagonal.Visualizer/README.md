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


### V2.5.1 canvas navigation polish

- Adds extra left breathing room so the first runtime node is never clipped.
- Adds click-and-drag panning on the flow-canvas background.
- Adds explicit START and END markers on the first and last runtime nodes.
- Keeps node clicks reserved for step selection.

## V2.5 scope

V2.5 keeps the three reference flows from V2 and adds a richer navigation/visual layer:

1. `UC-CRAFT-001 / CraftItem` — Command initiated by HTTP and persisted.
2. `UC-INVENTORY-001 / GetInventory` — Query initiated by HTTP with no state mutation.
3. `UC-SPOILAGE-001 / ProcessFoodSpoilage` — Background Command initiated by a Worker.

The UI compares each scenario by:

- flow type;
- trigger;
- whether it changes state;
- key architectural lesson;
- runtime call direction;
- general architectural stage;
- specific Hexagonal Architecture role;
- concrete .NET class/member;
- changing payload representation;
- relevant source lines read from the real `.cs` files;
- compile-time project dependency direction;
- active physical project/file.

The playback is an **educational simulation**, not runtime telemetry.

### V2.5 visual tooling

V2.5 adds:

- zoom controls over the runtime canvas;
- `Focus` mode to dim unrelated steps and emphasize the current boundary;
- `Follow` mode that keeps the active node visible while playback advances;
- an animated payload marker whose shape/color changes by payload category;
- a previous/current/next payload transformation rail;
- a compact role glossary for Exterior / Inbound / Core / Domain / Outbound;
- highlighted dependency edges around the active project;
- a synchronized physical source path: `src → project → folder → file → C# symbol`.

The visualizer still does not instrument or reference the productive projects.

## Run

From `implementations/dotnet`:

```powershell
dotnet run --project tools\Grounded.Hexagonal.Visualizer --launch-profile http
```

Or from this folder:

```powershell
dotnet run --launch-profile http
```

Open the URL printed by ASP.NET Core.

## What the scenarios teach

### CraftItem

Shows a state-changing Command:

`HTTP → Inbound Adapter → Input Port → Application → Domain → Output Port → Persistence Adapter`.

### GetInventory

Shows a read-only Query. It loads an `Inventory`, asks Domain for a read-only snapshot and returns a result without calling `SaveAsync`.

### ProcessFoodSpoilage

Shows that Hexagonal Architecture does not require HTTP as the entry mechanism. `FoodSpoilageWorker` is the inbound adapter. It also demonstrates `IClock → SystemClock`, an outbound port/adapter pair unrelated to database persistence.

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

## Important distinction

The upper graph shows **runtime call direction**.

The lower project graph shows **compile-time dependency direction**.

They are intentionally separate because runtime calls can travel toward an adapter even while source-code dependencies point back toward a port owned by the core.

## V2.5.2 — click or drag on the runtime map

The runtime canvas now distinguishes a click from a drag gesture:

- A short click on a runtime node selects that step.
- Moving the pointer more than a small threshold pans the canvas, even when the gesture starts on a node.
- A completed drag suppresses the synthetic click that browsers emit after pointer-up.
- Text selection is disabled inside the runtime canvas so dragging feels like moving a map instead of selecting labels.

This behavior is visualizer-only and does not affect the productive Hexagonal Architecture projects.
