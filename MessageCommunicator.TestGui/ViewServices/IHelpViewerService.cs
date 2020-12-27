using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IHelpViewerService : IViewService
    {
        Task ShowHelpPageAsync(string pageTitle);
    }
}
