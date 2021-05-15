using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using Avalonia.Styling;

namespace MessageCommunicator.TestGui
{
    public class FluentWindow<T> : ReactiveWindow<T>, IStyleable
        where T : OwnViewModelBase
    {
        Type IStyleable.StyleKey => typeof(Window);

        public FluentWindow()
        {
            this.ExtendClientAreaToDecorationsHint = true;
            this.ExtendClientAreaTitleBarHeightHint = -1;

            this.TransparencyLevelHint = WindowTransparencyLevel.None;

            this.GetObservable(WindowStateProperty)
                .Subscribe(x =>
                {
                    this.PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                    this.PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                });

            this.GetObservable(IsExtendedIntoWindowDecorationsProperty)
                .Subscribe(x =>
                {
                    if (!x)
                    {
                        this.SystemDecorations = SystemDecorations.Full;
                        this.TransparencyLevelHint = WindowTransparencyLevel.None;
                    }
                });
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);    
            
            this.ExtendClientAreaChromeHints = 
                Avalonia.Platform.ExtendClientAreaChromeHints.PreferSystemChrome |                 
                Avalonia.Platform.ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
    }
}
