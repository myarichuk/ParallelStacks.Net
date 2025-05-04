namespace ParallelStacks.Net;

public class ParallelStacksData
{
    public List<ThreadInfo> Threads { get; set; } = [];

    public List<TaskInfo> Tasks { get; set; } = [];

    public List<TaskRelationship> Relationships { get; set; } = [];
}