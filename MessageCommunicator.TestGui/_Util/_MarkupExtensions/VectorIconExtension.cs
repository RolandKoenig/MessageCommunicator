using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class VectorIconExtension : MarkupExtension
    {
        public object? Icon { get; set; }

        /// <inheritdoc />
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.Icon is Drawing iconAsDrawing)
            {
                return new DrawingPresenter()
                {
                    Drawing = iconAsDrawing,
                    Width = 16.0,
                    Height = 16.0,
                    Margin = new Thickness(2.0)
                };
            }
            else
            {
                return null;
            }
        }
    }
}
