using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices
{
    public interface ISaveFileViewService : IViewService
    {
        Task<string?> ShowSaveFileDialogAsync(IEnumerable<FileDialogFilter> filters, string defaultExtension);
    }
}
