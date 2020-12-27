using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpBrowserViewModel : OwnViewModelBase
    {
        private IntegratedDocRepository _docRepo;
        private IntegratedDocFile? _currentDocFile;

        public string LoadedDoc => _currentDocFile == null ? "" : _currentDocFile.ReadFullContent();

        public string WindowTitle =>
            _currentDocFile == null ? "Help Browser" : $"Help Browser - {_currentDocFile.Title}";

        public string PageTitle =>
            _currentDocFile == null ? "Empty" : $"Page {_currentDocFile.Title}";

        public IEnumerable<IntegratedDocFile> AllFiles => _docRepo.AllFiles;
        
        public IntegratedDocFile? CurrentDocFile
        {
            get => _currentDocFile;
            private set
            {
                if (_currentDocFile != value)
                {
                    _currentDocFile = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(this.LoadedDoc));
                    this.RaisePropertyChanged(nameof(this.WindowTitle));
                    this.RaisePropertyChanged(nameof(this.PageTitle));
                }
            }
        }
        
        public ReactiveCommand<object?, Unit> Command_Close { get; }
        
        public HelpBrowserViewModel(IntegratedDocRepository docRepo)
        {
            _docRepo = docRepo;

            this.Command_Close = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        public void NavigateTo(string docFileTitle)
        {
            this.CurrentDocFile = _docRepo.GetByTitle(docFileTitle);
        }
    }
}