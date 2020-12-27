using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpViewerService : ViewServiceBase, IHelpViewerService
    {
        private DialogHostControl _host;
        private IntegratedDocRepository _docRepo;

        public HelpViewerService(DialogHostControl host, IntegratedDocRepository docRepo)
        {
            _host = host;
            _docRepo = docRepo;
        }

        /// <inheritdoc />
        public Task ShowHelpPageAsync(string pageTitle)
        {
            var view = new HelpViewerControl();
            view.DataContext = new HelpViewerControlViewModel(pageTitle, _docRepo);

            return view.ShowControlDialogAsync(_host, "Documentation");
        }
    }
}
