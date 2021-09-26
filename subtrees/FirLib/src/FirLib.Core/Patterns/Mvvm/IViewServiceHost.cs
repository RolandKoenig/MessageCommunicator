using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.Mvvm
{
    public interface IViewServiceHost
    {
        public ICollection<IViewService> ViewServices { get; }

        public IViewServiceHost? ParentViewServiceHost { get; }

        public object? TryGetDefaultViewService(Type viewServiceType);
    }
}
