using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IAboutDialogService : IViewService
    {
        Task ShowAboutDialogAsync();
    }
}
