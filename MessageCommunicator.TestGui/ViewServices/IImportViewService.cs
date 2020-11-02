using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IImportViewService
    {
        Task ImportAsync<T>(ICollection<T> importTarget, string nameProperty, string dataTypeName)
            where T : class;
    }
}
