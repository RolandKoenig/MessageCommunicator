using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace MessageCommunicator.TestGui
{
    public class FluentWindowFrame : UserControl
    {
        private Window? _mainWindow;
        private Grid _ctrlFullWindowGrid;
        private Grid _ctrlMainGrid;
        private StackPanel _ctrlTitlePanel;
        private Panel _ctrlHeaderContent;
        private Panel _ctrlMainContent;
        private Panel _ctrlFooterContent;

        public Controls TitleContent => _ctrlTitlePanel.Children;
        public Controls HeaderContent => _ctrlHeaderContent.Children;
        public Controls MainContent => _ctrlMainContent.Children;
        public Controls FooterContent => _ctrlFooterContent.Children;

        public DialogHostControl DialogHostControl { get; }

        public FluentWindowFrame()
        {
            this.InitializeComponent();

            _ctrlFullWindowGrid = this.Find<Grid>("CtrlFullWindowGrid");
            _ctrlMainGrid = this.Find<Grid>("CtrlMainGrid");
            _ctrlTitlePanel = this.Find<StackPanel>("CtrlTitlePanel");
            _ctrlHeaderContent = this.Find<Panel>("CtrlHeader");
            _ctrlMainContent = this.Find<Panel>("CtrlMainContent");
            _ctrlFooterContent = this.Find<Panel>("CtrlFooter");
            this.DialogHostControl = this.Find<DialogHostControl>("CtrlDialogHost");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TryConfigureParentWindow()
        {
            if (_mainWindow == null) { return; }

            _mainWindow.ExtendClientAreaToDecorationsHint = true;
            _mainWindow.ExtendClientAreaTitleBarHeightHint = -1;
            _mainWindow.TransparencyLevelHint = WindowTransparencyLevel.None;
        }

        private void UpdateFrameState()
        {
            if (_mainWindow == null) { return; }

            // Configure window frame
            var useFullWindowMargin = false;
            var useTitlePanel = false;
            var useCenteredTitle = false;
            if (_mainWindow.IsExtendedIntoWindowDecorations)
            {
                useTitlePanel = true;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    switch (_mainWindow.WindowState)
                    {
                        case WindowState.FullScreen:
                        case WindowState.Maximized:
                            useFullWindowMargin = true;
                            break;
                    }
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    useTitlePanel = _mainWindow.WindowState != WindowState.FullScreen;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    useCenteredTitle = true;
                }
            }

            // Apply settings for content margin
            var titleBarMargin = 0.0;
            if (useFullWindowMargin)
            {
                _ctrlMainGrid.Margin = new Thickness(7.0, 0.0, 7.0, 7.0);
                _ctrlFullWindowGrid.Margin = new Thickness(0.0, 7.0, 0.0, 0.0);
                titleBarMargin = 7.0;
            }
            else
            {
                _ctrlMainGrid.Margin = new Thickness(0.0);
                _ctrlFullWindowGrid.Margin = new Thickness(0.0);
            }

            // Apply settings for title panel
            if (useTitlePanel)
            {
                _ctrlTitlePanel.IsVisible = true;
                _ctrlFullWindowGrid.RowDefinitions[0].Height = new GridLength(30.0);

                if (useCenteredTitle)
                {
                    _ctrlTitlePanel.Margin = new Thickness(0.0);
                    _ctrlTitlePanel.HorizontalAlignment = HorizontalAlignment.Center;
                    _ctrlTitlePanel.VerticalAlignment = VerticalAlignment.Center;
                }
                else
                {
                    _ctrlTitlePanel.Margin = new Thickness(7.0 + titleBarMargin, 0.0);
                    _ctrlTitlePanel.HorizontalAlignment = HorizontalAlignment.Left;
                    _ctrlTitlePanel.VerticalAlignment = VerticalAlignment.Center;
                }
            }
            else
            {
                _ctrlTitlePanel.IsVisible = false;
                _ctrlFullWindowGrid.RowDefinitions[0].Height = new GridLength(0.0);
            }
        }

        /// <inheritdoc />
        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);

            var newMainWindow = e.Parent as Window;
            if (!ReferenceEquals(newMainWindow, _mainWindow))
            {
                if (_mainWindow != null)
                {
                    _mainWindow.PropertyChanged -= this.OnMainWindow_PropertyChanged;
                }
                _mainWindow = newMainWindow;
                if (_mainWindow != null)
                {
                    _mainWindow.PropertyChanged += this.OnMainWindow_PropertyChanged;
                    this.TryConfigureParentWindow();
                }
            }

            // Trigger update of this control's state
            Dispatcher.UIThread.Post(this.UpdateFrameState);
        }

        private void OnMainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            this.UpdateFrameState();
        }
    }
}
