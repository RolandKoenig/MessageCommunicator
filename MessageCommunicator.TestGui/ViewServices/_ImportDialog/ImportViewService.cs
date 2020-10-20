using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportViewService : IImportViewService
    {
        private DialogHostControl _host;

        public ImportViewService(DialogHostControl host)
        {
            _host = host;
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> ImportAsync<T>(string nameProperty)
        {
            throw new NotImplementedException();
        }
    }
}
