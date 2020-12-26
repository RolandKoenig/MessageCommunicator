using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Markdown.Avalonia;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class HelpViewerControl : OwnUserControlDialog<OwnViewModelBase>
    {
        public HelpViewerControl()
        {
            AvaloniaXamlLoader.Load(this);

            var markdownViewer = this.FindControl<MarkdownScrollViewer>("CtrlMarkdownViewer");
            markdownViewer.Engine.BitmapLoader = new HelpBitmapLoader(
                Assembly.GetExecutingAssembly(),
                "Assets/Docs/");
        }
    }
}
