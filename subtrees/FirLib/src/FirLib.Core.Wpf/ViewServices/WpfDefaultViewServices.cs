using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirLib.Core.ViewServices;

internal static class WpfDefaultViewServices
{
    public static object? TryGetViewService(Window window, Type viewServiceType)
    {
        if(viewServiceType == typeof(IOpenFileViewService))
        {
            return new WpfOpenFileDialogService(window);
        }
        else if (viewServiceType == typeof(IOpenDirectoryViewService))
        {
            return new WpfOpenDirectoryViewService(window);
        }
        else if (viewServiceType == typeof(IMessageBoxService))
        {
            return new WpfMessageBoxService(window);
        }

        return TryGetViewService((UIElement) window, viewServiceType);
    }

    public static object? TryGetViewService(UIElement uiElement, Type viewServiceType)
    {
        return null;
    }
}