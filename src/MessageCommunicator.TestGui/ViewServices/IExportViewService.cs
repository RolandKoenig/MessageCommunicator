using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IExportViewService : IViewService
    {
        Task ExportAsync<T>(
            IEnumerable<T> allObjects, IEnumerable<T> objectsToExport, 
            string nameProperty, string dataTypeName)
            where T : class;
    }
}
