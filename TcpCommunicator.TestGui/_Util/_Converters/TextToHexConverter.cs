using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace TcpCommunicator.TestGui
{
    public class TextToHexConverter : IValueConverter
    {
        private StringBuilder? _cachedStringBuilder;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var givenEncodingName = parameter as string;
            if (parameter is Func<string?> parameterGetter)
            {
                givenEncodingName = parameterGetter();
            }

            if((value is string givenString) &&
               (!string.IsNullOrEmpty(givenEncodingName)))
            {
                try
                {
                    var givenEncoding = Encoding.GetEncoding(givenEncodingName);
                    var bytes = givenEncoding.GetBytes(givenString);
                    if (bytes.Length <= 0) { return string.Empty; }

                    if(_cachedStringBuilder == null){ _cachedStringBuilder = new StringBuilder(bytes.Length * 3 - 1); }
                    var strBuilder = _cachedStringBuilder;
                    strBuilder.Clear();
                    strBuilder.EnsureCapacity(bytes.Length * 3 - 1);

                    const string HEX_ALPHABET = "0123456789ABCDEF";
                    for (var loop = 0; loop < bytes.Length; loop++)
                    {
                        if (loop > 0) { strBuilder.Append(' '); }

                        var actByte = bytes[loop];
                        strBuilder.Append(HEX_ALPHABET[actByte >> 4]);
                        strBuilder.Append(HEX_ALPHABET[actByte & 0xF]);
                    }
                    return strBuilder.ToString();
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
            return string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
            //if ((value is string givenString) &&
            //    (parameter is string givenEncodingName))
            //{
            //    try
            //    {
                    
            //        const string HEX_ALPHABET = "0123456789ABCDEF";
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //}
        }
    }
}
