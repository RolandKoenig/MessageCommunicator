using Avalonia.Controls;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ViewResourceService : ViewServiceBase, IViewResourceService
    {
        private IResourceNode _rootNode;
        
        public ViewResourceService(IResourceNode resourceNode)
        {
            _rootNode = resourceNode;
        }

        /// <inheritdoc />
        public object? TryGetViewResource(string resourceName)
        {
            return _rootNode.FindResource(resourceName);
        }
    }
}