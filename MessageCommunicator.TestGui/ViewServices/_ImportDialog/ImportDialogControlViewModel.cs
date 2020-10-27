using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using Force.DeepCloner;
using MessageCommunicator.TestGui.Data;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportDialogControlViewModel<T> : OwnViewModelBase
    {
        public ImportDialogControlViewModel(ICollection<T> importTarget, string nameProperty)
        {

        }
    }
}
