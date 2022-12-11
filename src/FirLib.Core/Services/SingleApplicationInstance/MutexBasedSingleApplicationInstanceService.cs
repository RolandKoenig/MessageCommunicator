using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FirLib.Core.Services.SingleApplicationInstance;

public class MutexBasedSingleApplicationInstanceService : ISingleApplicationInstanceService, IDisposable
{
    private Mutex _mutex;
    private bool _mutexOwned;

    /// <inheritdoc />
    public bool IsMainInstance => _mutexOwned;

    /// <inheritdoc />
    public bool CanSendReceiveMessages => false;

    /// <inheritdoc />
#pragma warning disable CS0067
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
#pragma warning restore CS0067

    public MutexBasedSingleApplicationInstanceService(string mutexName)
    {
        _mutex = new Mutex(true, mutexName, out _mutexOwned);
    }

    /// <inheritdoc />
    public bool TrySendMessageToMainInstance(string message)
    {
        return false;
    }

    public void Dispose()
    {
        _mutex.Dispose();
    }
}