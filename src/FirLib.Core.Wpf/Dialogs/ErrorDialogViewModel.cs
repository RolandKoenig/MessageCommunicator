using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns;
using FirLib.Core.Patterns.ErrorAnalysis;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.Dialogs;

public class ErrorDialogViewModel : ViewModelBase
{
    public ExceptionInfo ExceptionInfo { get; }

    public DelegateCommand Command_Close { get; }

    public DelegateCommand Command_ReportError { get; }

    public ErrorDialogViewModel(ExceptionInfo exInfo)
    {
        this.ExceptionInfo = exInfo;

        this.Command_Close = new DelegateCommand(() => this.CloseWindow(true));
        this.Command_ReportError = new DelegateCommand(() => throw new NotImplementedException());
    }
}