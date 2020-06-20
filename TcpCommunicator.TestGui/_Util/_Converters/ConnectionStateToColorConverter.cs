using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Tmds.DBus;

namespace TcpCommunicator.TestGui
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
