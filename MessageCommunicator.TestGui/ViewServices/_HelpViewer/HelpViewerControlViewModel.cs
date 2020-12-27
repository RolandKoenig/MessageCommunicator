using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpViewerControlViewModel : OwnViewModelBase
    {
        public string LoadedDoc { get; }

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public HelpViewerControlViewModel(string pageTitle, IntegratedDocRepository docRepo)
        {
            this.LoadedDoc = docRepo.GetByTitle(pageTitle).ReadFullContent();

            this.Command_OK = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }
    }
}
