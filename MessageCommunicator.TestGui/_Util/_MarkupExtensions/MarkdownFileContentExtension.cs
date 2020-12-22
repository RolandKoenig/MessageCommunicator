using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui
{
    public class MarkdownFileContentExtension : MarkupExtension
    {
        public string? MarkdownFileName { get; set; }

        /// <inheritdoc />
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(this.MarkdownFileName))
            {
                return null;
            }

            try
            {
                return IntegratedDocUtil.Current.GetIntegratedDoc(this.MarkdownFileName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
