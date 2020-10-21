using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class SaveFileDialogService : ISaveFileViewService
    {
        private Window _parent;

        public SaveFileDialogService(Window parent)
        {
            _parent = parent;
        }

        /// <inheritdoc />
        public Task<string?> ShowSaveFileDialogAsync(IEnumerable<FileDialogFilter> filters, string defaultExtension)
        {
            var dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filters.AddRange(filters);
            dlgSaveFile.DefaultExtension = defaultExtension;

            return dlgSaveFile.ShowAsync(_parent);
        }
    }
}
