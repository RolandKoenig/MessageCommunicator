using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class DialogHostControl : Grid
    {
        public static readonly DirectProperty<DialogHostControl, Control?> OccludedControlProperty =
            AvaloniaProperty.RegisterDirect<DialogHostControl, Control?>(
                nameof(OccludedControl),
                o => o.OccludedControl,
                (o, v) => o.OccludedControl = v);

        private Control? _currentChild;
        private Control? _currentChildBorder;
        private Size _currentChildInitialSize;
        private Control? _occludedControl;
        private readonly IBrush _backgroundDialog;
        private readonly IBrush _backgroundNoDialog;

        public Control? OccludedControl
        {
            get { return _occludedControl; }
            set { this.SetAndRaise(OccludedControlProperty, ref _occludedControl, value); }
        }

        public DialogHostControl()
        {
            _backgroundNoDialog = Brushes.Transparent;
            _backgroundDialog = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            this.Background = _backgroundNoDialog;
            this.IsHitTestVisible = false;

        }

        public void ShowDialog(Control controlToShow, string headerText)
        {
            if (_currentChild != null)
            {
                throw new ApplicationException($"Unable to show multiple controls on {nameof(DialogHostControl)}");
            }

            _currentChild = controlToShow;
            _currentChildInitialSize = new Size(_currentChild.Width, _currentChild.Height);

            var borderControl = new HeaderedContentControl();
            borderControl.Classes.Add("DialogHostControlBorder");
            borderControl.BorderThickness = new Thickness(1.0);
            borderControl.Content = _currentChild;
            borderControl.HorizontalAlignment = HorizontalAlignment.Center;
            borderControl.VerticalAlignment = VerticalAlignment.Center;
            borderControl.Padding = new Thickness(5.0);
            borderControl.Header = headerText;
            borderControl.Classes.Add("DialogBox");
            _currentChildBorder = borderControl;

            this.Background = _backgroundDialog;
            this.Children.Add(borderControl);
            this.IsHitTestVisible = true;

            if (this.OccludedControl != null)
            {
                this.OccludedControl.IsEnabled = false;
            }

            this.UpdateBorderSize();
        }

        public void CloseDialog()
        {
            if (_currentChild == null) { return; }

            this.Children.Clear();
            _currentChild = null;
            _currentChildBorder = null;
            _currentChildInitialSize = Size.Empty;

            this.Background = _backgroundNoDialog;
            this.IsHitTestVisible = false;

            if (this.OccludedControl != null)
            {
                this.OccludedControl.IsEnabled = true;
            }
        }

        private void UpdateBorderSize()
        {
            if (_currentChild == null) { return; }
            if (_currentChildBorder == null){ return; }

            const double BORDER_PADDING = 50.0;

            // Update height
            if (this.Bounds.Height < _currentChildInitialSize.Height + BORDER_PADDING)
            {
                _currentChild.Height = this.Bounds.Height - BORDER_PADDING;
            }
            else
            {
                _currentChild.Height = _currentChildInitialSize.Height;
            }

            // Update width
            if (this.Bounds.Width < _currentChildInitialSize.Width + BORDER_PADDING)
            {
                _currentChild.Width = this.Bounds.Width - BORDER_PADDING;
            }
            else
            {
                _currentChild.Width = _currentChildInitialSize.Width;
            }
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == Grid.BoundsProperty)
            {
                this.UpdateBorderSize();
            }
        }
    }
}
