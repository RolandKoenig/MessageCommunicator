using System;
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
        event EventHandler<ViewServiceRequestEventArgs>? ViewServiceRequest;
    }

    public enum IconBrushStyle
    {
        Positive,

        Negative,
        
        NegativeSoft,

        Neutral,
    }

    public enum MainWindowFrameStatus
    {
        NeutralGray,

        NeutralBlue,

        Green,

        Yellow,

        Red
    }
}
