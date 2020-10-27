using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportViewService : IImportViewService
    {
        private DialogHostControl _host;

        public ImportViewService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public async Task ImportAsync<T>(ICollection<T> importTarget, string nameProperty)
        {
            var importDlg = new ImportDialogControl();
            importDlg.DataContext = new ImportDialogControlViewModel<T>(importTarget, nameProperty);

            await importDlg.ShowControlDialogAsync(_host, "Import");
        }
    }
}
