using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public interface IOpenFileViewService : IViewService
{
    Task<string?> ShowOpenFileDialogAsync(IReadOnlyList<FileDialogFilter> filters, string title);

    Task<string[]?> ShowOpenMultipleFilesDialogAsync(IReadOnlyList<FileDialogFilter> filters, string title);
}