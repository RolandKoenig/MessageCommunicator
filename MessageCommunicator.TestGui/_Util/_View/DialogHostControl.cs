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

            var borderControl = new HeaderedContentControl();
            borderControl.Classes.Add("DialogHostControlBorder");
            borderControl.BorderThickness = new Thickness(1.0);
            borderControl.Content = _currentChild;
            borderControl.HorizontalAlignment = HorizontalAlignment.Center;
            borderControl.VerticalAlignment = VerticalAlignment.Center;
            borderControl.Padding = new Thickness(5.0);
            borderControl.Header = headerText;
            borderControl.Classes.Add("DialogBox");

            this.Background = _backgroundDialog;
            this.Children.Add(borderControl);
            this.IsHitTestVisible = true;

            if (this.OccludedControl != null)
            {
                this.OccludedControl.IsEnabled = false;
            }
        }

        public void CloseDialog()
        {
            if (_currentChild == null) { return; }

            this.Children.Clear();
            _currentChild = null;

            this.Background = _backgroundNoDialog;
            this.IsHitTestVisible = false;

            if (this.OccludedControl != null)
            {
                this.OccludedControl.IsEnabled = true;
            }
        }
    }
}
