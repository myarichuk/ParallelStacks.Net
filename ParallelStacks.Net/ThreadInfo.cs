namespace ParallelStacks.Net;

public class ThreadInfo
{
    public int OsThreadId { get; set; }
    public List<StackFrameInfo> StackFrames { get; set; } = [];
}