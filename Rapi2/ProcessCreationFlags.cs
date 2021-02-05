using System;

namespace Rapi2
{
    /// <summary>
    /// Flags that control the priority and the creation of the process. 
    /// </summary>
    [Flags]
    public enum ProcessCreationFlags
    {
        /// <summary>No conditions are set on the created process.</summary>
        None = 0,
        /// <summary>For Windows CE versions 2.0 and later. Calling process is treated as a debugger, and the new process is a process being debugged. Child processes of the new process are also debugged. The system notifies the debugger of all debug events that occur in the process being debugged.</summary>
        DebugProcess = 0x00000001,
        /// <summary>For Windows CE versions 2.0 and later. Calling process is treated as a debugger, and the new process is a process being debugged. No child processes of the new process are debugged. The system notifies the debugger of all debug events that occur in the process being debugged.</summary>
        DebugOnlyThisProcess = 0x00000002,
        /// <summary>The primary thread of the new process is created in a suspended state.</summary>
        CreateSuspended = 0x00000004,
        /// <summary>For Windows CE versions 3.0 and later. The new process has a new console, instead of inheriting the parent's console.</summary>
        CreateNewConsole = 0x00000010
    }
}