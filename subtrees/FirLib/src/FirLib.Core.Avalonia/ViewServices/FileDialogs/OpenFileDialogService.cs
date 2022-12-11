using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices.FileDialogs;

public class OpenFileDialogService : ViewServiceBase, IOpenFileViewService
{
    private Window _parent;

    public OpenFileDialogService(Window parent)
    {
        _parent = parent;
    }

    /// <inheritdoc />
    public async Task<string?> ShowOpenFileDialogAsync(IReadOnlyList<FileDialogFilter> filters, string defaultExtension)
    {
        var dlgOpenFile = new OpenFileDialog();

        if (filters.Count > 0)
        {
            dlgOpenFile.Filters ??= new List<global::Avalonia.Controls.FileDialogFilter>(filters.Count);
            foreach (var actFilter in filters)
            {
                var actAvaloniaFilter = new global::Avalonia.Controls.FileDialogFilter();
                actAvaloniaFilter.Name = actFilter.Name;
                actAvaloniaFilter.Extensions = actFilter.Extensions;
                dlgOpenFile.Filters.Add(actAvaloniaFilter);
            }
        }

        dlgOpenFile.AllowMultiple = false;

        var selectedFiles = await dlgOpenFile.ShowAsync(_parent);
        if ((selectedFiles == null) ||
            (selectedFiles.Length == 0))
        {
            return null;
        }
        else
        {
            return selectedFiles[0];
        }
    }

    /// <inheritdoc />
    public async Task<string[]?> ShowOpenMultipleFilesDialogAsync(IReadOnlyList<FileDialogFilter> filters, string title)
    {
        var dlgSaveFile = new OpenFileDialog();
        
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

        dlgSaveFile.AllowMultiple = true;

        var selectedFiles = await dlgSaveFile.ShowAsync(_parent);
        if ((selectedFiles == null) ||
            (selectedFiles.Length == 0))
        {
            return null;
        }
        else
        {
            return selectedFiles;
        }
    }
}