using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.Mvvm;

public interface IViewService
{
    event EventHandler<ViewServiceRequestEventArgs>? ViewServiceRequest;
}