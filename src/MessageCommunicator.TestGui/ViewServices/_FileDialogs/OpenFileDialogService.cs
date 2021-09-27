using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class OpenFileDialogService : ViewServiceBase, IOpenFileViewService
    {
        private Window _parent;

        public OpenFileDialogService(Window parent)
        {
            _parent = parent;
        }

        /// <inheritdoc />
        public async Task<string?> ShowOpenFileDialogAsync(IEnumerable<FileDialogFilter> filters, string defaultExtension)
        {
            var dlgSaveFile = new OpenFileDialog();
            dlgSaveFile.Filters.AddRange(filters);
            dlgSaveFile.AllowMultiple = false;

            var selectedFiles = await dlgSaveFile.ShowAsync(_parent);
            if ((selectedFiles == null) ||
                (selectedFiles.Length == 0))
            {
                return null;
            }
            else
            {
                return selectedFiles[0];
            }
        }
    }
}
