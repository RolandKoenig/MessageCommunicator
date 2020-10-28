using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IExportViewService
    {
        Task ExportAsync<T>(IEnumerable<T> allObjects, IEnumerable<T> objectsToExport, string nameProperty, string dataTypeName);
    }
}
