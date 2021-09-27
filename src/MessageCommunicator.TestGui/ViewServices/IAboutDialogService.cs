using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IAboutDialogService : IViewService
    {
        Task ShowAboutDialogAsync();
    }
}
