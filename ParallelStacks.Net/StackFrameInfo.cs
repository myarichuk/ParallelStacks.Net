namespace ParallelStacks.Net;

public record StackFrameInfo
{
    public string MethodName { get; init; }
    public string ModuleName { get; init; }
    public ulong InstructionPointer { get; init; }
}