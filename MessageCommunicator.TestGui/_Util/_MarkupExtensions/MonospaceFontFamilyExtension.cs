using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class MonospaceFontFamilyExtension : MarkupExtension
    {
        private static FontFamily? s_monospaceFontFamily;

        /// <inheritdoc />
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            return GetDefaultMonospaceFont();
        }

        public static FontFamily? GetDefaultMonospaceFont()
        {
            if (s_monospaceFontFamily == null)
            {
                s_monospaceFontFamily =
                    TryParseFontFamily("DejaVu Sans Mono")
                    ?? TryParseFontFamily("Consolas")
                    ?? TryParseFontFamily("Courier New");
            }

            return s_monospaceFontFamily;
        }

        public static FontFamily? TryParseFontFamily(string fontFamilyName)
        {
            try
            {
                return FontFamily.Parse(fontFamilyName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
