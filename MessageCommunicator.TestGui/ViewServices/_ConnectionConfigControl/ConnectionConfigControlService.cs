using System.Threading.Tasks;
using Avalonia.Controls;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControlService : IConnectionConfigViewService
    {
        private DialogHostControl _host;

        public ConnectionConfigControlService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public async Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template)
        {
            var configDlg = new ConnectionConfigControl();
            configDlg.DataContext = new ConnectionConfigControlViewModel(template);

            return await configDlg.ShowControlDialogAsync(_host) as ConnectionParameters;
        }
    }
}
