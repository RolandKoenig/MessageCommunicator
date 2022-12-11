using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FirLib.Core.Wpf.Controls.CustomWindow;

/// <summary>
/// </summary>
public class CustomWindowTitleBar : Control
{
    public static readonly DependencyProperty IconSourceProperty;
    public static readonly DependencyProperty MaximizeButtonVisibilityProperty;
    public static readonly DependencyProperty RestoreButtonVisibilityProperty;

    private Window? _parentWindow;

    public ImageSource IconSource
    {
        get => (ImageSource) GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public Visibility MaximizeButtonVisibility
    {
        get => (Visibility) GetValue(MaximizeButtonVisibilityProperty);
        set => SetValue(MaximizeButtonVisibilityProperty, value);
    }

    public Visibility RestoreButtonVisibility
    {
        get => (Visibility) GetValue(RestoreButtonVisibilityProperty);
        set => SetValue(RestoreButtonVisibilityProperty, value);
    }

    static CustomWindowTitleBar()
    {
        IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(CustomWindowTitleBar), new PropertyMetadata(default(ImageSource)));
        MaximizeButtonVisibilityProperty = DependencyProperty.Register(
            "MaximizeButtonVisibility", typeof(Visibility), typeof(CustomWindowTitleBar), new PropertyMetadata(Visibility.Visible));
        RestoreButtonVisibilityProperty = DependencyProperty.Register(
            "RestoreButtonVisibility", typeof(Visibility), typeof(CustomWindowTitleBar), new PropertyMetadata(Visibility.Visible));

        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CustomWindowTitleBar), 
            new FrameworkPropertyMetadata(typeof(CustomWindowTitleBar)));
    }

    public CustomWindowTitleBar()
    {
        _parentWindow = null;
    }

    private Window? FindWindow(DependencyObject currentElement)
    {
        var actParent = currentElement;
        while (actParent != null)
        {
            if (actParent is Window window) { return window; }

            actParent = VisualTreeHelper.GetParent(actParent);
        }
        return null;
    }

    private void UpdateControlState()
    {
        if (_parentWindow == null) { return; }

        if (_parentWindow.WindowState == WindowState.Maximized)
        {
            this.RestoreButtonVisibility = Visibility.Visible;
            this.MaximizeButtonVisibility = Visibility.Collapsed;
        }
        else
        {
            this.RestoreButtonVisibility = Visibility.Collapsed;
            this.MaximizeButtonVisibility = Visibility.Visible;
        }
    }

    /// <inheritdoc />
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        if (_parentWindow != null)
        {
            _parentWindow.StateChanged -= this.OnParentWindow_StateChanged;
        }
        _parentWindow = this.FindWindow(this);
        if (_parentWindow != null)
        {
            _parentWindow.StateChanged += this.OnParentWindow_StateChanged;
        }

        this.UpdateControlState();
    }

    private void OnParentWindow_StateChanged(object? sender, EventArgs e)
    {
        this.UpdateControlState();
    }
}