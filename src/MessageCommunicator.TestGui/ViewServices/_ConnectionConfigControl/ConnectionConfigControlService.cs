using System.Collections.Generic;
using System.Threading.Tasks;
using FirLib.Core.Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControlService : ViewServiceBase, IConnectionConfigViewService
    {
        private DialogHostControl _host;

        public ConnectionConfigControlService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public async Task<ConnectionParameters?> ConfigureConnectionAsync(
            ConnectionParameters? template, 
            IEnumerable<ConnectionParameters> allConnectionsParameters)
        {
            var dialogTitle = template == null ? "Create Profile" : "Edit Profile";

            var configDlg = new ConnectionConfigControl();
            configDlg.DataContext = new ConnectionConfigControlViewModel(template, allConnectionsParameters);

            return await configDlg.ShowControlDialogAsync(_host, dialogTitle) as ConnectionParameters;
        }
    }
}
