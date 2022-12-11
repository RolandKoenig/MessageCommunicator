using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core;

public enum ActionIfSyncContextIsNull
{
    InvokeSynchronous,

    InvokeUsingNewTask,

    DontInvoke
}