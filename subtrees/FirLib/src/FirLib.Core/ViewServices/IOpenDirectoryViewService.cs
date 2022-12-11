using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public interface IOpenDirectoryViewService : IViewService
{
    Task<string?> ShowOpenDirectoryDialogAsync(string title);
}