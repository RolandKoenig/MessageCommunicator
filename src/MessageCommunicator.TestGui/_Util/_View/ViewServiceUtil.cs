using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui
{
    public static class ViewServiceUtil
    {
        public static T FindViewService<T>(this IControl thisControl)
            where T : class
        {
            return (T)TryFindViewService(thisControl, typeof(T))!; 
        }

        public static T? TryFindViewService<T>(this IControl thisControl)
            where T : class
        {
            return TryFindViewService(thisControl, typeof(T)) as T;
        }

        public static object? TryFindViewService(this IControl thisControl, Type viewServiceType)
        {
            var actParent = thisControl;
            object? result = null;
            while (actParent != null)
            {
                if (actParent is IViewServiceHost viewServiceHost)
                {
                    foreach (var actViewService in viewServiceHost.ViewServices)
                    {
                        if(actViewService == null){ continue; }

                        // ReSharper disable once UseMethodIsInstanceOfType
                        if(!viewServiceType.IsAssignableFrom(actViewService.GetType())){ continue; }

                        result = actViewService;
                    }
                }

                actParent = actParent.Parent;
            }
            return result;
        }
    }
}
