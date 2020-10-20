using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IImportViewService
    {
        Task<IEnumerable<T>> ImportAsync<T>(string nameProperty);
    }
}
