using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FirLib.Core.Wpf.Controls.CustomWindow
{
    public partial class CustomWindowBase : Window
    {
        public static readonly DependencyProperty TitleBarHeightProperty;
        public static readonly DependencyProperty TitleBarButtonForegroundProperty;
        public static readonly DependencyProperty TitleBarButtonBackgroundHoverProperty;
        public static readonly DependencyProperty TitleBarButtonBackgroundHitProperty;

        public ICommand Command_Minimize { get; }

        public ICommand Command_Maximize { get; }

        public ICommand Command_Restore { get; }

        public ICommand Command_Close { get; }

        public double TitleBarHeight
        {
            get { return (double) GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        public Brush TitleBarButtonForeground
        {
            get { return (Brush) GetValue(TitleBarButtonForegroundProperty); }
            set { SetValue(TitleBarButtonForegroundProperty, value); }
        }

        public Brush TitleBarButtonBackgroundHit
        {
            get { return (Brush) GetValue(TitleBarButtonBackgroundHitProperty); }
            set { SetValue(TitleBarButtonBackgroundHitProperty, value); }
        }

        public Brush TitleBarButtonBackgroundHover
        {
            get { return (Brush) GetValue(TitleBarButtonBackgroundHoverProperty); }
            set { SetValue(TitleBarButtonBackgroundHoverProperty, value); }
        }

        static CustomWindowBase()
        {
            TitleBarHeightProperty = DependencyProperty.Register(
                "TitleBarHeight", typeof(double), typeof(CustomWindowBase), new PropertyMetadata(32.0));
            TitleBarButtonForegroundProperty = DependencyProperty.Register(
                "TitleBarButtonForeground", typeof(Brush), typeof(CustomWindowBase), new PropertyMetadata(default(Brush)));
            TitleBarButtonBackgroundHoverProperty = DependencyProperty.Register(
                "TitleBarButtonBackgroundHover", typeof(Brush), typeof(CustomWindowBase), new PropertyMetadata(default(Brush)));
            TitleBarButtonBackgroundHitProperty = DependencyProperty.Register(
                "TitleBarButtonBackgroundHit", typeof(Brush), typeof(CustomWindowBase), new PropertyMetadata(default(Brush)));

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomWindowBase), 
                new FrameworkPropertyMetadata(typeof(CustomWindowBase)));
        }

        public CustomWindowBase()
        {
            this.Command_Minimize = new CustomWindowCommand(
                () => this.WindowState = WindowState.Minimized);
            this.Command_Maximize = new CustomWindowCommand(
                () => this.WindowState = WindowState.Maximized,
                () => this.WindowState != WindowState.Maximized);
            this.Command_Restore = new CustomWindowCommand(
                () => this.WindowState = WindowState.Normal,
                () => this.WindowState == WindowState.Maximized);
            this.Command_Close = new CustomWindowCommand(this.Close);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            ((CustomWindowCommand)this.Command_Maximize).RaiseCanExecuteChanged();
            ((CustomWindowCommand)this.Command_Restore).RaiseCanExecuteChanged();
        }
    }
}
