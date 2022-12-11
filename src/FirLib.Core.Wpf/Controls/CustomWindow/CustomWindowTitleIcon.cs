using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FirLib.Core.Wpf.Controls.CustomWindow;

public class CustomWindowTitleIcon : Control
{
    static CustomWindowTitleIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindowTitleIcon), new FrameworkPropertyMetadata(typeof(CustomWindowTitleIcon)));
    }
}