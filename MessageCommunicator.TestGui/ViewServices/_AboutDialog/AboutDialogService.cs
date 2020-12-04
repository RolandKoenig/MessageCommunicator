using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class AboutDialogService : ViewServiceBase, IAboutDialogService
    {
        private DialogHostControl _host;

        public AboutDialogService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public Task ShowAboutDialogAsync()
        {
            var aboutDialogControl = new AboutDialogControl();
            aboutDialogControl.DataContext = new AboutDialogViewModel();
            return aboutDialogControl.ShowControlDialogAsync(_host, "About");
        }
    }
}
