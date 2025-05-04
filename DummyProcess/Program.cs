// File: Program.cs (in ParallelStacks.Net.TestHelper project)

namespace ParallelStacks.Net.TestHelper;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            await Console.Error.WriteLineAsync("Missing mode switch.");
            Environment.Exit(1);
        }

        switch (args[0].ToLowerInvariant())
        {
            case "hold-task":
                await HoldTaskAsync();
                break;

            case "sleep":
                SleepForever();
                break;

            default:
                await Console.Error.WriteLineAsync($"Unknown mode: {args[0]}");
                Environment.Exit(2);
                break;
        }
    }

    private static async Task HoldTaskAsync()
    {
        var tcs = new TaskCompletionSource<object?>();
        _ = tcs.Task; // Prevent compiler warning

        Console.WriteLine("Task created and waiting.");
        await Task.Delay(-1); // block forever (simulate long-lived await)
    }

    private static void SleepForever()
    {
        Console.WriteLine("Sleeping forever.");
        Thread.Sleep(Timeout.Infinite);
    }
}