using System.Text;

namespace ParallelStacks.Net;

public class MermaidFormatter : IParallelStacksFormatter
{
    public string Format(ParallelStacksData d)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph TD");
        foreach (var t in d.Tasks) sb.AppendLine($"  Task{t.TaskId}[\"Task {t.TaskId} ({t.Status})\"]");

        foreach (var rel in d.Relationships)
            sb.AppendLine($"  Task{rel.FromTaskId} -->|{rel.Type}| Task{rel.ToTaskId}");

        return sb.ToString();
    }
}