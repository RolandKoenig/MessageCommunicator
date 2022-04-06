using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace MessageCommunicator.TestGui
{
    public class TextToHexConverter : IValueConverter
    {
        private StringBuilder? _cachedStringBuilder;

        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var givenEncodingName = parameter as string;
            if (parameter is Func<string?> parameterGetter)
            {
                givenEncodingName = parameterGetter();
            }

            if ((value is string givenString) &&
                (!string.IsNullOrEmpty(givenString)) &&
                (!string.IsNullOrEmpty(givenEncodingName)))
            {
                try
                {
                    // Parse hex string to byte array
                    var targetByteArray = new byte[givenString.Length / 2 + 1];
                    var targetByteArrayPos = 0;
                    for (var loop = 0; loop < givenString.Length; loop++)
                    {
                        if(givenString[loop] == ' '){ continue; }

                        ReadOnlySpan<char> currentSpan;
                        if ((givenString.Length > loop + 1) &&
                            (givenString[loop + 1] != ' '))
                        {
                            currentSpan = givenString.AsSpan(loop, 2);
                        }
                        else
                        {
                            currentSpan = givenString.AsSpan(loop, 1);
                        }

                        if (!byte.TryParse(currentSpan, NumberStyles.AllowHexSpecifier, null, out var currentByte))
                        {
                            break;
                        }

                        targetByteArray[targetByteArrayPos] = currentByte;
                        targetByteArrayPos++;

                        loop += (currentSpan.Length - 1);
                    }
                    
                    // Convert byte array to result value
                    if (targetByteArrayPos > 0)
                    {
                        var givenEncoding = Encoding.GetEncoding(givenEncodingName);
                        return givenEncoding.GetString(targetByteArray, 0, targetByteArrayPos);
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            }

            return string.Empty;
        }
    }
}
