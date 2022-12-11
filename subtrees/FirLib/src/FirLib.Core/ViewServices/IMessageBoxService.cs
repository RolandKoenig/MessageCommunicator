using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.ViewServices;

public interface IMessageBoxService : IViewService
{
    Task<MessageBoxResult> ShowAsync(string title, string message, MessageBoxButtons buttons);
}