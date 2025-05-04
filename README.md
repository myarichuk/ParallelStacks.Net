# ParallelStacks.Net

**ParallelStacks.Net** is a .NET 8+ library for inspecting live async and multithreaded state using ClrMD, similar to Visual Studio's Parallel Stacks view.

## Features

- Capture async `Task` and thread info from live or dump processes
- Reconstruct logical relationships between tasks (e.g., awaits, continuations)
- Output call graph to multiple formats (e.g. Mermaid, GraphViz)

## Getting Started

```bash
dotnet add package ParallelStacks.Net
```

```csharp
using ParallelStacks.Net;

// Attach to a process and get stack data
var collector = new ParallelStacksCollector(pid);
var data = collector.Collect();

// Format it
var mermaid = new MermaidFormatter().Format(data);
Console.WriteLine(mermaid);
```

## License

MIT
