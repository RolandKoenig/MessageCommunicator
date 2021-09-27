using Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

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
            if (_rootNode.TryGetResource(resourceName, out var foundRes)) { return foundRes; }
            else { return null;}
        }
    }
}