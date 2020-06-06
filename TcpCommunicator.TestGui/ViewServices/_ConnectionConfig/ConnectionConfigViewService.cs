using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.ViewServices
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
            configDlg.DataContext = new ConnectionConfigViewModel(template);

            return await configDlg.ShowDialog<ConnectionParameters?>(_parentWindow);
        }
    }
}
