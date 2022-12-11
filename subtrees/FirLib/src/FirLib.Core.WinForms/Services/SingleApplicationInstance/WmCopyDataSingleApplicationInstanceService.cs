using System;
using System.Windows.Forms;

namespace FirLib.Core.Services.SingleApplicationInstance;

public class WmCopyDataSingleApplicationInstanceService : ISingleApplicationInstanceService, IDisposable
{
    private IntPtr _targetWindow;
    private CopyDataControl? _ctrlCopyData;

    /// <inheritdoc />
    public bool IsMainInstance { get; }

    /// <inheritdoc />
    public bool CanSendReceiveMessages => true;

    /// <inheritdoc />
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    public WmCopyDataSingleApplicationInstanceService(string controlName)
    {
        _targetWindow = NativeMethods.FindWindow(null, controlName);
        if (_targetWindow == IntPtr.Zero)
        {
            this.IsMainInstance = true;

            CreateParams createParams = new();
            createParams.Caption = controlName;
            createParams.X = 0;
            createParams.Y = 0;
            createParams.Width = 100;
            createParams.Height = 100;
            createParams.Parent = IntPtr.Zero;

            _ctrlCopyData = new();
            _ctrlCopyData.CreateHandle(createParams);
            _ctrlCopyData.MessageReceived += this.OnCtrlCopyData_MessageReceived;
        }
        else
        {
            this.IsMainInstance = false;
        }
    }

    private void OnCtrlCopyData_MessageReceived(object? sender, MessageReceivedEventArgs e)
    {
        this.MessageReceived?.Invoke(this, e);
    }

    /// <inheritdoc />
    public bool TrySendMessageToMainInstance(string message)
    {
        if (_targetWindow == IntPtr.Zero) { return false; }
        if (this.IsMainInstance || (_ctrlCopyData != null)) { return false; }
            
        CopyDataControl.Send(_targetWindow, message);
        return true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _ctrlCopyData?.DestroyHandle();
        _ctrlCopyData = null;
        _targetWindow = IntPtr.Zero;
    }
}