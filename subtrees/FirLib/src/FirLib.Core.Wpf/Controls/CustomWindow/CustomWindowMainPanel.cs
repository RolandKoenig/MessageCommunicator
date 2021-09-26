using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace FirLib.Core.Wpf.Controls.CustomWindow
{
    [ContentProperty(nameof(CustomWindowMainPanel.Content))]
    public class CustomWindowMainPanel : Control
    {
        public static readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty IconSourceProperty;

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public ImageSource IconSource
        {
            get => (ImageSource) GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        static CustomWindowMainPanel()
        {
            ContentProperty = DependencyProperty.Register(
                "Content", typeof(object), typeof(CustomWindowMainPanel), new PropertyMetadata(default(object)));
            IconSourceProperty = DependencyProperty.Register(
                "IconSource", typeof(ImageSource), typeof(CustomWindowMainPanel), new PropertyMetadata(default(ImageSource)));

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomWindowMainPanel), 
                new FrameworkPropertyMetadata(typeof(CustomWindowMainPanel)));
        }
    }
}
