using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TcpCommunicator.TestGui.Views
{
    public class LoggingView : OwnUserControl<LoggingViewModel>
    {
        private DataGridColumn _columnType;

        public bool IsTypeColumnVisible
        {
            get => _columnType.IsVisible;
            set => _columnType.IsVisible = value;
        }

        public LoggingView()
        {
            AvaloniaXamlLoader.Load(this);

            var ctrlDataGrid = this.FindControl<DataGrid>("CtrlDataGrid");
            _columnType = ctrlDataGrid.Columns[1];
        }


    }
}
