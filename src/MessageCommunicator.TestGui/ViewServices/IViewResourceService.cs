using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IViewResourceService : IViewService
    {
        object? TryGetViewResource(string resourceName);
    }
}