using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace MessageCommunicator.TestGui
{
    public class EncodingWebNameToDescriptionConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string encodingWebName)) { return "-"; }

            try
            {
                return $"({Encoding.GetEncoding(encodingWebName).EncodingName})";
            }
            catch (Exception)
            {
                return "-";
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
