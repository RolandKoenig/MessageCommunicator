using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class MonospaceFontExtension : MarkupExtension
    {
        private static FontFamily? s_monospaceFontFamily;

        public MonospaceFontValue Value { get; set; } = MonospaceFontValue.FontFamily;

        /// <inheritdoc />
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            switch (this.Value)
            {
                case MonospaceFontValue.FontFamily:
                    return GetDefaultMonospaceFont();

                case MonospaceFontValue.FontSize:
                    return 14.0;

                default:
                    throw new NotSupportedException(
                        $"Value {this.Value} from enum {nameof(MonospaceFontValue)} not supported!");
            }
        }

        public static FontFamily? GetDefaultMonospaceFont()
        {
            s_monospaceFontFamily = new FontFamily("avares://MessageCommunicator.TestGui/Assets/Fonts#Inconsolata");
            return s_monospaceFontFamily;
        }
    }
}
