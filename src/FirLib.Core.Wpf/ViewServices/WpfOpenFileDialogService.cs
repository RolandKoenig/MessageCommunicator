using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirLib.Core.Patterns.Mvvm;
using Microsoft.Win32;

namespace FirLib.Core.ViewServices;

internal class WpfOpenFileDialogService : ViewServiceBase, IOpenFileViewService
{
    private Window? _owner;

    public WpfOpenFileDialogService(Window? owner)
    {
        _owner = owner;
    }

    /// <inheritdoc />
    public Task<string?> ShowOpenFileDialogAsync(IEnumerable<FileDialogFilter> filters, string title)
    {
        var dlgOpenFile = new OpenFileDialog();
        dlgOpenFile.Title = title;
        dlgOpenFile.Filter = FileDialogFilter.BuildFilterString(filters);
        dlgOpenFile.Multiselect = false;
        if (dlgOpenFile.ShowDialog(_owner) == true)
        {
            return Task.FromResult<string?>(dlgOpenFile.FileName);
        }
        else
        {
            return Task.FromResult<string?>(null);
        }
    }

    /// <inheritdoc />
    public Task<string[]?> ShowOpenMultipleFilesDialogAsync(IEnumerable<FileDialogFilter> filters, string title)
    {
        var dlgOpenFile = new OpenFileDialog();
        dlgOpenFile.Title = title;
        dlgOpenFile.Filter = FileDialogFilter.BuildFilterString(filters);
        dlgOpenFile.Multiselect = true;
        if (dlgOpenFile.ShowDialog(_owner) == true)
        {
            return Task.FromResult<string[]?>(dlgOpenFile.FileNames);
        }
        else
        {
            return Task.FromResult<string[]?>(null);
        }
    }
}