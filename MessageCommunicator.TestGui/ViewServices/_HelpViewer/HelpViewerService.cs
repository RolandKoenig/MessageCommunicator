using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpViewerService : ViewServiceBase, IHelpViewerService
    {
        private DialogHostControl _host;

        public HelpViewerService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public Task ShowHelpPageAsync(string helpPageResourceFile)
        {
            var view = new HelpViewerControl();
            view.DataContext = new HelpViewerControlViewModel(helpPageResourceFile);

            return view.ShowControlDialogAsync(_host, "Documentation");
        }
    }
}
