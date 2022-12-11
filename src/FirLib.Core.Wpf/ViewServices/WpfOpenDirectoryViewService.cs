using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public class WpfOpenDirectoryViewService : ViewServiceBase, IOpenDirectoryViewService
{
    private Window _owner;

    public WpfOpenDirectoryViewService(Window owner)
    {
        _owner = owner;
    }

    /// <inheritdoc />
    public Task<string?> ShowOpenDirectoryDialogAsync(string title)
    {
        using (var dialog = new FolderBrowserDialog())
        {
            dialog.Description = title;
            if(dialog.ShowDialog(_owner.GetIWin32Window()) == DialogResult.OK)
            {
                return Task.FromResult<string?>(dialog.SelectedPath);
            }
            else
            {
                return Task.FromResult<string?>(null);
            }
        }
    }
}