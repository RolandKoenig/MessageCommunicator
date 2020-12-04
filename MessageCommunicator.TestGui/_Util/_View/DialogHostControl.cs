using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        private Stack<ChildInfo> _children;
        private Control? _occludedControl;
        private readonly IBrush _backgroundDialog;

        public Control? OccludedControl
        {
            get { return _occludedControl; }
            set { this.SetAndRaise(OccludedControlProperty, ref _occludedControl, value); }
        }

        public DialogHostControl()
        {
            _children = new Stack<ChildInfo>();
            _backgroundDialog = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            this.IsHitTestVisible = false;

        }

        public void ShowDialog(Control controlToShow, string headerText)
        {
            this.ShowDialogAsync(controlToShow, headerText);
        }

        public Task ShowDialogAsync(Control controlToShow, string headerText)
        {
            var currentChild = controlToShow;
            var currentChildInitialSize = new Size(currentChild.Width, currentChild.Height);

            var borderControl = new HeaderedContentControl();
            borderControl.Classes.Add("DialogHostControlBorder");
            borderControl.BorderThickness = new Thickness(1.0);
            borderControl.Content = currentChild;
            borderControl.HorizontalAlignment = HorizontalAlignment.Center;
            borderControl.VerticalAlignment = VerticalAlignment.Center;
            borderControl.Padding = new Thickness(5.0);
            borderControl.Header = headerText;
            borderControl.Classes.Add("DialogBox");
            var currentChildBorder = borderControl;

            var currentBackground = new Grid();
            currentBackground.Background = _backgroundDialog;

            var taskComplSource = new TaskCompletionSource<object?>();
            _children.Push(new ChildInfo(
                currentChild, currentChildBorder, 
                currentChildInitialSize, currentBackground,
                taskComplSource));

            this.Children.Add(currentBackground);
            this.Children.Add(borderControl);
            this.IsHitTestVisible = true;

            if (this.OccludedControl != null)
            {
                this.OccludedControl.IsEnabled = false;
            }

            this.UpdateBorderSize();

            return taskComplSource.Task;
        }

        public void CloseDialog()
        {
            if (!_children.TryPop(out var removedChild))
            {
                return;
            }
            this.Children.Remove(removedChild.ChildBorder);
            this.Children.Remove(removedChild.ChildBackground);
            removedChild.TaskComplSource.TrySetResult(null);

            if (_children.Count == 0)
            {
                this.IsHitTestVisible = false;

                if (this.OccludedControl != null)
                {
                    this.OccludedControl.IsEnabled = true;
                }
            }
        }

        private void UpdateBorderSize()
        {
            const double BORDER_PADDING = 50.0;

            foreach (var actChildInfo in _children)
            {
                var currentChild = actChildInfo.Child;
                var currentChildInitialSize = actChildInfo.InitialSize;

                // Update height
                if (this.Bounds.Height < currentChildInitialSize.Height + BORDER_PADDING)
                {
                    currentChild.Height = this.Bounds.Height - BORDER_PADDING;
                }
                else
                {
                    currentChild.Height = currentChildInitialSize.Height;
                }

                // Update width
                if (this.Bounds.Width < currentChildInitialSize.Width + BORDER_PADDING)
                {
                    currentChild.Width = this.Bounds.Width - BORDER_PADDING;
                }
                else
                {
                    currentChild.Width = currentChildInitialSize.Width;
                }
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

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        public class ChildInfo
        {
            public Control Child
            {
                get;
            }

            public Control ChildBorder
            {
                get;
            }

            public Grid ChildBackground
            {
                get;
            }

            public Size InitialSize
            {
                get;
            }

            public TaskCompletionSource<object?> TaskComplSource
            {
                get;
            }

            internal ChildInfo(
                Control child, Control childBorder, 
                Size initialSize, Grid childBackground,
                TaskCompletionSource<object?> taskComplSource)
            {
                this.Child = child;
                this.ChildBorder = childBorder;
                this.InitialSize = initialSize;
                this.ChildBackground = childBackground;
                this.TaskComplSource = taskComplSource;
            }
        }
    }
}
