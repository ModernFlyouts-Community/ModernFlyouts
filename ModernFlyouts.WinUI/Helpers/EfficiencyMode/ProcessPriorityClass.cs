namespace ModernFlyouts.EfficiencyMode;
/// <summary>
/// Based on: <br/>
/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-setpriorityclass#parameters"/>
/// </summary>
public enum ProcessPriorityClass
{
    /// <summary>
    /// Process with no special scheduling needs.
    /// </summary>
    Default,

    /// <summary>
    /// Process whose threads run only when the system is idle. 
    /// The threads of the process are preempted by the threads of any process running in a 
    /// higher priority class. An example is a screen saver. 
    /// The idle-priority class is inherited by child processes.
    /// </summary>
    Idle,

    /// <summary>
    /// Process that has priority above IDLE_PRIORITY_CLASS but below NORMAL_PRIORITY_CLASS.
    /// </summary>
    BelowNormal,

    /// <summary>
    /// Process with no special scheduling needs.
    /// </summary>
    Normal,

    /// <summary>
    /// Process that has priority above NORMAL_PRIORITY_CLASS but below HIGH_PRIORITY_CLASS.
    /// </summary>
    AboveNormal,

    /// <summary>
    /// Process that performs time-critical tasks that must be executed immediately.
    /// The threads of the process preempt the threads of normal or idle priority class processes. 
    /// An example is the Task List, which must respond quickly when called by the user, 
    /// regardless of the load on the operating system. Use extreme care when using the high-priority class, 
    /// because a high-priority class application can use nearly all available CPU time.
    /// </summary>
    High,

    /// <summary>
    /// Process that has the highest possible priority. 
    /// The threads of the process preempt the threads of all other processes, 
    /// including operating system processes performing important tasks. 
    /// For example, a real-time process that executes for more than a very brief interval 
    /// can cause disk caches not to flush or cause the mouse to be unresponsive.
    /// </summary>
    Realtime,
}
