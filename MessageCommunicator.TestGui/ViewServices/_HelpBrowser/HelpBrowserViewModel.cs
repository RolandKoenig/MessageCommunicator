using System.Reactive;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpBrowserViewModel : OwnViewModelBase
    {
        private IntegratedDocRepository _docRepo;
        
        public string LoadedDoc { get; private set; }
        
        public string WindowTitle { get; private set; }
        
        public ReactiveCommand<object?, Unit> Command_Close { get; }
        
        public HelpBrowserViewModel(IntegratedDocRepository docRepo)
        {
            _docRepo = docRepo;
            this.LoadedDoc = string.Empty;
            this.WindowTitle = "Help Browser";
            
            this.Command_Close = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        public void NavigateTo(string docFileTitle)
        {
            this.LoadedDoc = _docRepo.GetByTitle(docFileTitle).ReadFullContent();
            this.RaisePropertyChanged(nameof(this.LoadedDoc));

            this.WindowTitle = $"Help Browser - {docFileTitle}";
            this.RaisePropertyChanged(nameof(this.WindowTitle));
        }
    }
}