using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MessageCommunicator.TestGui
{
    public interface IViewServiceHost
    {
        public ICollection<IViewService> ViewServices { get; }
    }

    public interface IViewService
    {
        
    }
}
