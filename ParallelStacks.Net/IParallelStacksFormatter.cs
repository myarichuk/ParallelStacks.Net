namespace ParallelStacks.Net;

public interface IParallelStacksFormatter
{
    /// <summary>
    ///     Turn the raw data into a string render
    /// </summary>
    string Format(ParallelStacksData data);
}