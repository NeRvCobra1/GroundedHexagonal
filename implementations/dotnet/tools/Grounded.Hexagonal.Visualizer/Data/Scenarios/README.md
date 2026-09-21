# Scenario metadata

Each JSON file describes one educational playback without referencing the productive assemblies.

The visualizer deliberately uses curated metadata rather than runtime instrumentation. A step maps:

1. a runtime action;
2. its general architectural stage;
3. its Hexagonal Architecture role;
4. the concrete .NET type/member;
5. an optional source-code anchor.

`SourceSnippetService` resolves the referenced file underneath `implementations/dotnet` and locates the current lines at runtime. This keeps snippets synchronized with the real source without copying productive code into the visualizer.

Future scenarios should follow the same model as `CraftItem.json`.
