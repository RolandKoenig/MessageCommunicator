using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IOpenFileViewService
    {
        Task<string?> ShowOpenFileDialogAsync(IEnumerable<FileDialogFilter> filters, string defaultExtension);
    }
}
