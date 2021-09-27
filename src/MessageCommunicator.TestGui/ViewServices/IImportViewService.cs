using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IImportViewService : IViewService
    {
        Task<ImportResult<T>?> ImportAsync<T>(ICollection<T> importTarget, string nameProperty, string dataTypeName)
            where T : class;
    }
}
