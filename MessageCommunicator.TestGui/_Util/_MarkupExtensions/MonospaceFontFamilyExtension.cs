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
        private static string? s_monospaceFontFamily;

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetDefaultMonospaceFont();
        }

        public static string GetDefaultMonospaceFont()
        {
            if (s_monospaceFontFamily == null)
            {
                if (IsFontFamilyAvailable("DejaVu Sans Mono")) { s_monospaceFontFamily = "DejaVu Sans Mono"; }
                else if (IsFontFamilyAvailable("Consolas")) { s_monospaceFontFamily = "Consolas"; }
                else
                {
                    s_monospaceFontFamily = "Courier New";
                }
            }

            return s_monospaceFontFamily;
        }

        public static bool IsFontFamilyAvailable(string fontFamilyName)
        {
            try
            {
                FontFamily.Parse(fontFamilyName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
