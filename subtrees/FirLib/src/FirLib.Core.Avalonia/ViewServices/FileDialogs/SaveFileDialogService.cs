using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices.FileDialogs;

public class SaveFileDialogService : ViewServiceBase, ISaveFileViewService
{
    private Window _parent;

    public SaveFileDialogService(Window parent)
    {
        _parent = parent;
    }

    /// <inheritdoc />
    public Task<string?> ShowSaveFileDialogAsync(IReadOnlyList<FileDialogFilter> filters, string defaultExtension)
    {
        var dlgSaveFile = new SaveFileDialog();

        if (filters.Count > 0)
        {
            dlgSaveFile.Filters ??= new List<global::Avalonia.Controls.FileDialogFilter>(filters.Count);
            foreach (var actFilter in filters)
            {
                var actAvaloniaFilter = new global::Avalonia.Controls.FileDialogFilter();
                actAvaloniaFilter.Name = actFilter.Name;
                actAvaloniaFilter.Extensions = actFilter.Extensions;
                dlgSaveFile.Filters.Add(actAvaloniaFilter);
            }
        }

        dlgSaveFile.DefaultExtension = defaultExtension;

        return dlgSaveFile.ShowAsync(_parent);
    }
}