namespace MessageCommunicator.TestGui.ViewServices
{
    public interface IViewResourceService : IViewService
    {
        object? TryGetViewResource(string resourceName);
    }
}