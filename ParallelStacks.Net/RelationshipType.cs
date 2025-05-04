namespace ParallelStacks.Net;

public enum RelationshipType
{
    Awaits, // A --> B because A is awaiting B
    ContinuesWith // A --> B because B is registered as a continuation of A
}