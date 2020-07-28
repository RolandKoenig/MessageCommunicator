using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class ConnectionStateToColorConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connState = (ConnectionState) value;
            switch (connState)
            {
                case ConnectionState.Stopped:
                    return Brushes.Gray;

                case ConnectionState.Connected:
                    return Brushes.Green;
                
                case ConnectionState.Connecting:
                    return Brushes.Yellow;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
