using System.Collections.Generic;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public interface ISaveFileViewService : IViewService
{
    Task<string?> ShowSaveFileDialogAsync(IReadOnlyList<FileDialogFilter> filters, string defaultExtension);
}