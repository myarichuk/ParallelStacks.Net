using System.Diagnostics;

namespace ParallelStacks.Net.Tests;

public class Basics
{
    [Fact]
    public void Collect_Returns_TaskInfo_From_Helper()
    {
        var helperPath = Path.Combine(
            AppContext.BaseDirectory,
            "DummyProcess.dll");
        Assert.True(File.Exists(helperPath), $"Expected helper not found at: {helperPath}");

        var proc = Process.Start(new ProcessStartInfo("dotnet", $"\"{helperPath}\" hold-task")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        try
        {
            Assert.NotNull(proc);
            Thread.Sleep(1000); // Give it time to spin up

            var collector = new ParallelStacksCollector(proc.Id);
            var data = collector.Collect();

            Assert.NotEmpty(data.Tasks);
        }
        finally
        {
            proc.Kill(true);
            proc.WaitForExit(1000);
            proc?.Dispose();
        }
    }

    [Fact]
    public void Collect_Returns_ThreadInfo_From_Helper()
    {
        var helperPath = Path.Combine(
            AppContext.BaseDirectory,
            "DummyProcess.dll");
        Assert.True(File.Exists(helperPath), $"Expected helper not found at: {helperPath}");

        var proc = Process.Start(new ProcessStartInfo("dotnet", $"\"{helperPath}\" sleep")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        try
        {
            Assert.NotNull(proc);
            Thread.Sleep(1000); // Give it time to spin up

            var collector = new ParallelStacksCollector(proc.Id);
            var data = collector.Collect();

            Assert.NotEmpty(data.Threads);
        }
        finally
        {
            proc.Kill(true);
            proc.WaitForExit(1000);
            proc?.Dispose();
        }
    }
}