using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public class WpfMessageBoxService : ViewServiceBase, IMessageBoxService
{
    private Window _owner;

    public WpfMessageBoxService(Window owner)
    {
        _owner = owner;
    }

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons)
    {
        switch (buttons)
        {
            case MessageBoxButtons.Ok:
                return Task.FromResult(FromWpfMessageBoxResult(
                    MessageBox.Show(message, title, MessageBoxButton.OK)));

            case MessageBoxButtons.OkCancel:
                return Task.FromResult(FromWpfMessageBoxResult(
                    MessageBox.Show(message, title, MessageBoxButton.OKCancel)));

            case MessageBoxButtons.YesNo:
                return Task.FromResult(FromWpfMessageBoxResult(
                    MessageBox.Show(message, title, MessageBoxButton.YesNo)));

            case MessageBoxButtons.YesNoCancel:
                return Task.FromResult(FromWpfMessageBoxResult(
                    MessageBox.Show(message, title, MessageBoxButton.YesNoCancel)));

            default:
                throw new ArgumentException($"MessageBoxButtons {buttons} not handled!");
        }
    }

    private static MessageBoxResult FromWpfMessageBoxResult(System.Windows.MessageBoxResult result)
    {
        switch (result)
        {
            case System.Windows.MessageBoxResult.None:
            case System.Windows.MessageBoxResult.Cancel:
                return MessageBoxResult.Cancel;

            case System.Windows.MessageBoxResult.Yes:
                return MessageBoxResult.Yes;

            case System.Windows.MessageBoxResult.No:
                return MessageBoxResult.No;

            case System.Windows.MessageBoxResult.OK:
                return MessageBoxResult.Ok;

            default:
                throw new ArgumentException($"System.Windows.MessageBoxResult {result} not handled!");
        }
    }
}