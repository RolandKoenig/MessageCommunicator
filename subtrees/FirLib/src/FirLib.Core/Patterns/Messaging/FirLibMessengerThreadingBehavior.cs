using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Patterns.Messaging;

/// <summary>
/// This enum describes the threading behavior of a Messenger.
/// </summary>
public enum FirLibMessengerThreadingBehavior
{
    /// <summary>
    /// Ignore threading. Just use default lock on registration list.
    /// </summary>
    Ignore,

    /// <summary>
    /// Ensures that the main SynchronizationContext is set when 
    /// this Messenger is used on synchronous calls.
    /// </summary>
    EnsureMainSyncContextOnSyncCalls
}