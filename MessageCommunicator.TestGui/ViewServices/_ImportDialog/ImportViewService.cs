using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportViewService : IImportViewService
    {
        private DialogHostControl _host;
        private IOpenFileViewService _srvOpenFile;
        private IMessageBoxService _srvMessageBox;

        public ImportViewService(DialogHostControl host, IOpenFileViewService srvOpenFile, IMessageBoxService srvMessageBox)
        {
            _host = host;
            _srvOpenFile = srvOpenFile;
            _srvMessageBox = srvMessageBox;
        }

        /// <inheritdoc />
        public async Task ImportAsync<T>(ICollection<T> importTarget, string nameProperty, string dataTypeName)
        {
            var fileToImport = await _srvOpenFile.ShowOpenFileDialogAsync(
                new[]
                {
                    new FileDialogFilter()
                    {
                        Name = "Data-Package (*.dataPackage)",
                        Extensions = {"dataPackage"}
                    }
                }, "dataPackage");

            if (string.IsNullOrEmpty(fileToImport))
            {
                await _srvMessageBox.ShowAsync("Import", "No file selected!", MessageBoxButtons.Ok);
                return;
            }


            var importDlg = new ImportDialogControl();
            importDlg.DataContext = new ImportDialogControlViewModel<T>(importTarget, nameProperty, dataTypeName);

            await importDlg.ShowControlDialogAsync(_host, "Import");
        }
    }
}
