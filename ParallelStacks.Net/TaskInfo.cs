namespace ParallelStacks.Net;

public record TaskInfo
{
    public int? TaskId { get; init; } // if TCS might be null
    public TaskStatus Status { get; init; }
    public List<StackFrameInfo> StackFrames { get; set; } = [];
}