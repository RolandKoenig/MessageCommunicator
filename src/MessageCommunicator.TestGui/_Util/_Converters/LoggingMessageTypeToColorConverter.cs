using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class LoggingMessageTypeToColorConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connState = (string) value;
            switch (connState)
            {
                case nameof(LoggingMessageType.Error):
                    return Brushes.Red;

                case nameof(LoggingMessageType.Warning):
                    return Brushes.Yellow;
                
                default:
                    return Brushes.Transparent;
            }
        }

        /// <inheritdoc />
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
