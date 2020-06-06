using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace TcpCommunicator.TestGui
{
    public class ConnectionConfigViewService : IConnectionConfigViewService
    {
        private Window _parentWindow;

        public ConnectionConfigViewService(Window parentWindow)
        {
            _parentWindow = parentWindow;
        }

        /// <inheritdoc />
        public async Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template)
        {
            var configDlg = new ConnectionConfigView();
            configDlg.ViewModel = new ConnectionConfigViewModel(template);
            configDlg.DataContext = configDlg.ViewModel;

            return await configDlg.ShowDialog<ConnectionParameters?>(_parentWindow);
        }
    }
}
