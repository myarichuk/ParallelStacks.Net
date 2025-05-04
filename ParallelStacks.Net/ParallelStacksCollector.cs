using Microsoft.Diagnostics.Runtime;

namespace ParallelStacks.Net;

public class ParallelStacksCollector : IDisposable
{
    private readonly DataTarget _dt;
    private readonly ClrRuntime _runtime;

    public ParallelStacksCollector(int processId, int timeoutMs = 5000)
    {
        // note - CreateSnapshotAndAttach is not supported on macOS
        _dt = processId == Environment.ProcessId
            ? DataTarget.CreateSnapshotAndAttach(processId)
            :
            // ClrMD doesn't work with unsuspended processes
            DataTarget.AttachToProcess(processId, true);

        var version = _dt.ClrVersions[0];
        _runtime = version.CreateRuntime();
    }

    public void Dispose()
    {
        _runtime?.Dispose();
        _dt?.Dispose();
    }

    public ParallelStacksData Collect()
    {
        var result = new ParallelStacksData();

        foreach (var th in _runtime.Threads.Where(t => t.IsAlive))
        {
            var ti = new ThreadInfo
            {
                OsThreadId = (int)th.OSThreadId,
                StackFrames = th.EnumerateStackTrace()
                    .Select(f => new StackFrameInfo
                    {
                        MethodName = f.Method?.Signature ?? $"0x{f.InstructionPointer:x}",
                        ModuleName = f.Method?.Type?.Module.Name ?? "<unknown>",
                        InstructionPointer = f.InstructionPointer
                    })
                    .ToList()
            };
            result.Threads.Add(ti);
        }

        var heap = _runtime.Heap;

        bool IsTaskType(ClrType type)
        {
            for (var t = type; t != null; t = t.BaseType)
                if (t.Name.StartsWith("System.Threading.Tasks.Task"))
                    return true;

            return false;
        }

        foreach (var obj in heap.EnumerateObjects())
        {
            var clrType = obj.Type;
            if (clrType == null)
                continue;

            ClrObject taskObject;
            if (IsTaskType(clrType))
            {
                // this object is Task or Task<TResult>
                taskObject = heap.GetObject(obj.Address);
            }
            else if (clrType.Name?.StartsWith("System.Threading.Tasks.TaskCompletionSource") ??
                     throw new InvalidOperationException("WTF? A type without a name?!"))
            {
                // unwrap TaskCompletionSource<T> to the actual Task<T>
                var tcsObj = heap.GetObject(obj.Address);
                var taskAddr = tcsObj.ReadField<ulong>("m_task");
                if (taskAddr == 0) continue;

                taskObject = heap.GetObject(taskAddr);

                // ensure the unwrapped object is a Task
                if (taskObject.Type == null || !IsTaskType(taskObject.Type))
                    continue;
            }
            else
            {
                // not a Task or TCS
                continue;
            }

            // a real Task instance!
            int id;
            var idField = taskObject.Type.Fields
                .FirstOrDefault(f => f.Name.Equals("m_taskId", StringComparison.OrdinalIgnoreCase)
                                     || f.Name.Equals("_taskId", StringComparison.OrdinalIgnoreCase));
            if (idField != null)
                id = taskObject.ReadField<int>(idField.Name);
            else
                continue; // we have a TCS so NOP

            int state;
            var stateField = taskObject.Type.Fields
                .FirstOrDefault(f => f.Name.IndexOf("stateFlags", StringComparison.OrdinalIgnoreCase) >= 0);
            state = stateField != null
                ? taskObject.ReadField<int>(stateField.Name)
                : 0;

            var info = new TaskInfo
            {
                TaskId = id,
                Status = (TaskStatus)state
            };

            // Optional: inspect its continuation box
            var boxAddr = taskObject.ReadField<ulong>("m_action");
            if (boxAddr != 0 && heap.GetObjectType(boxAddr) is ClrType boxType)
                info.StackFrames = ExtractFramesFromBox(boxAddr, boxType);

            result.Tasks.Add(info);
        }

        result.Relationships = BuildTaskRelationships(result.Tasks);

        return result;
    }

    private List<StackFrameInfo> ExtractFramesFromBox(ulong boxAddr, ClrType boxType)
    {
        // TODO:
        //  - find the IAsyncStateMachine instance
        //  - peek its MoveNext() call stack via a separate DataTarget.Attach call
        return [];
    }

    private List<TaskRelationship> BuildTaskRelationships(IEnumerable<TaskInfo> tasks)
    {
        // TODO:
        // - scan each TaskInfo, look for continuation registration
        // - parse out the "target task id" from state machine fields, etc.
        // - map those to RelationshipType.Awaits or ContinuesWith.
        return [];
    }
}