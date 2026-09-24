# Scenario metadata

Each JSON file describes one educational playback without referencing productive assemblies.

V2 currently includes:

- `CraftItem.json`
- `GetInventory.json`
- `ProcessFoodSpoilage.json`

A scenario contains comparison metadata (`flowType`, `trigger`, `changesState`, `keyLesson`) plus ordered runtime steps.

Each step maps:

1. a runtime action;
2. its general architectural stage;
3. its Hexagonal Architecture role;
4. the concrete .NET type/member;
5. the payload representation at that instant;
6. an optional source-code anchor.

`SourceSnippetService` resolves referenced files underneath `implementations/dotnet` and locates current lines at runtime. This keeps snippets synchronized with the real source without copying productive code into the visualizer.

The metadata is intentionally curated. It is an educational map, not automatic runtime tracing.
