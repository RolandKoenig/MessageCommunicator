using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IConnectionConfigViewService : IViewService
    {
        Task<ConnectionParameters?> ConfigureConnectionAsync(ConnectionParameters? template, IEnumerable<ConnectionParameters> allConnections);
    }
}
