using System;
using System.Windows;
using FirLib.Core.Patterns;

namespace FirLib.Core;

public static partial class FirLibExtensionsWpf
{
    public static IDisposable HandleDataContextChanged<TViewModelType>(
        this FrameworkElement frameworkElement,
        Action<TViewModelType> attachNewViewModel,
        Action<TViewModelType> detachOldViewModel)
        where TViewModelType : class
    {
        if (frameworkElement.DataContext is TViewModelType existingViewModel)
        {
            attachNewViewModel(existingViewModel);
        }

        void ChangedEventHandler(object _, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is TViewModelType oldViewModel) { detachOldViewModel(oldViewModel); }
            if (args.NewValue is TViewModelType newViewModel) { attachNewViewModel(newViewModel); }
        }

        frameworkElement.DataContextChanged += ChangedEventHandler;
        return new DummyDisposable(() => frameworkElement.DataContextChanged -= ChangedEventHandler);
    }
}