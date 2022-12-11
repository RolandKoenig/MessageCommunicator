using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.Mvvm;

namespace FirLib.Core.Dialogs;

public partial class ErrorDialog : MvvmWindow
{
    public ErrorDialog()
    {
        this.InitializeComponent();
    }
}