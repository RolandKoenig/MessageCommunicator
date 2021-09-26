using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices
{
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
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = title;
                if(dialog.ShowDialog(_owner.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
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
}
