using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class DialogHostControl : Grid
    {
        private Control? _currentChild;
        private readonly IBrush _backgroundDialog;
        private readonly IBrush _backgroundNoDialog;

        public DialogHostControl()
        {
            _backgroundNoDialog = Brushes.Transparent;
            _backgroundDialog = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            this.Background = _backgroundNoDialog;
            this.IsHitTestVisible = false;
        }

        public void ShowDialog(Control controlToShow)
        {
            if (_currentChild != null)
            {
                throw new ApplicationException($"Unable to show multiple controls on {nameof(DialogHostControl)}");
            }

            _currentChild = controlToShow;

            var borderControl = new Border();
            borderControl.Classes.Add("DialogHostControlBorder");
            borderControl.BorderThickness = new Thickness(1.0);
            borderControl.Child = _currentChild;
            borderControl.HorizontalAlignment = HorizontalAlignment.Center;
            borderControl.VerticalAlignment = VerticalAlignment.Center;
            borderControl.Padding = new Thickness(5.0);

            this.Background = _backgroundDialog;
            this.Children.Add(borderControl);
            this.IsHitTestVisible = true;
        }

        public void CloseDialog()
        {
            if (_currentChild == null) { return; }

            this.Children.Clear();
            _currentChild = null;

            this.Background = _backgroundNoDialog;
            this.IsHitTestVisible = false;
        }
    }
}
