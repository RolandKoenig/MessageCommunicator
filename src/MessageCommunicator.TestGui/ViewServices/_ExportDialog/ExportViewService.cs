using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ExportViewService : ViewServiceBase, IExportViewService
    {
        private DialogHostControl _host;

        public ExportViewService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public async Task ExportAsync<T>(IEnumerable<T> allObjects, IEnumerable<T> objectsToExport, string nameProperty, string dataTypeName)
            where T : class
        {
            var exportDlg = new ExportDialogControl();
            exportDlg.DataContext = new ExportDialogControlViewModel<T>(allObjects, objectsToExport, nameProperty, dataTypeName);

            await exportDlg.ShowControlDialogAsync(_host, "Export");
        }
    }
}
