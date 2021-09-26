using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirLib.Core.Wpf.Controls.CustomWindow
{
    public class CustomWindowTitleIcon : Control
    {
        static CustomWindowTitleIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindowTitleIcon), new FrameworkPropertyMetadata(typeof(CustomWindowTitleIcon)));
        }
    }
}
