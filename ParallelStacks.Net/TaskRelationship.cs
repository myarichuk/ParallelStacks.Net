namespace ParallelStacks.Net;

public class TaskRelationship
{
    public int FromTaskId { get; set; }
    public int ToTaskId { get; set; }
    public RelationshipType Type { get; set; }
}