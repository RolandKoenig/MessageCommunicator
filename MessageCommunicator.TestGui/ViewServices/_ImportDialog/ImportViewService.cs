using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportViewService : ViewServiceBase, IImportViewService
    {
        private DialogHostControl _host;

        public ImportViewService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public async Task<ImportResult<T>?> ImportAsync<T>(ICollection<T> importTarget, string nameProperty, string dataTypeName)
            where T : class
        {
            var srvOpenFile = this.GetViewService<IOpenFileViewService>();
            var srvMessageBox = this.GetViewService<IMessageBoxService>();

            // Choose file to import
            var fileToImport = await srvOpenFile.ShowOpenFileDialogAsync(
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
                await srvMessageBox.ShowAsync("Import", "No file selected!", MessageBoxButtons.Ok);
                return null;
            }

            // Import the file
            List<T> importedLines;
            using (var dataPackage = new DataPackageFile(fileToImport, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    importedLines = dataPackage.ReadSingleFile<List<T>>(dataTypeName);
                }
                catch (Exception ex)
                {
                    await srvMessageBox.ShowAsync("Import", $"Error while importing: {ex.Message}", MessageBoxButtons.Ok);
                    return null;
                }
            }

            var importDlg = new ImportDialogControl();
            importDlg.DataContext = new ImportDialogControlViewModel<T>(importTarget, importedLines, nameProperty, dataTypeName);

            return await importDlg.ShowControlDialogAsync(_host, "Import") 
                as ImportResult<T>;
        }
    }
}
