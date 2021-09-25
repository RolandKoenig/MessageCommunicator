using System;
using System.Collections.Generic;
using System.Text;
using Light.GuardClauses;

namespace MessageCommunicator.Util
{
    public static class HexFormatUtil
    {
        private const string HEX_ALPHABET = "0123456789ABCDEF";

        public static string ToHexString(byte[] bytes)
        {
            return ToHexString(new ArraySegment<byte>(bytes, 0, bytes.Length));
        }

        public static string ToHexString(ArraySegment<byte> bytes)
        {
            bytes.MustNotBeDefault(nameof(bytes));

            if (bytes.Count == 0) { return string.Empty; }

            var length = bytes.Count;
            if (length > 1) { length += (length - 1); }
            var stringBuffer = StringBuffer.Acquire(length);
            var bytesSpan = bytes.AsSpan();
            try
            {
                for (var loop = 0; loop < bytesSpan.Length; loop++)
                {
                    if(stringBuffer.Count > 0){ stringBuffer.Append(' '); }

                    var actByte = bytesSpan[loop];
                    stringBuffer.Append(HEX_ALPHABET[actByte >> 4]);
                    stringBuffer.Append(HEX_ALPHABET[actByte & 0xF]);
                }

                return stringBuffer.ToString();
            }
            finally
            {
                StringBuffer.Release(stringBuffer);
            }
        }

        public static unsafe byte[] ToByteArray(string hexString)
        {
            hexString.MustNotBeNull(nameof(hexString));

            // Count hex characters
            var hexCharCount = 0;
            for (var loop = 0; loop < hexString.Length; loop++)
            {
                if(hexString[loop] == ' '){ continue; }
                hexCharCount++;
            }
            if (hexCharCount == 0) { return new byte[0]; }
            if (hexCharCount % 2 != 0)
            {
                throw new ArgumentException("Provided uneven count of hex characters!", nameof(hexString));
            }

            // Parse all bytes
            var result = new byte[hexCharCount / 2];
            var resultPos = 0;
            var hexPos = 0;
            var hexValues = stackalloc int[2];
            for (var loop = 0; loop < hexString.Length; loop++)
            {
                if(hexString[loop] == ' '){ continue; }

                var asciiValue = (int) (hexString[loop]);
                if ((asciiValue > 47) && (asciiValue < 58))
                {
                    // 0, 1, 2...
                    hexValues[hexPos] = asciiValue - 48;
                }
                else if ((asciiValue > 64) && (asciiValue < 71))
                {
                    // A, B, C...
                    hexValues[hexPos] = (asciiValue - 65) + 10;
                }
                else if ((asciiValue > 96) && (asciiValue < 103))
                {
                    // a, b, c...
                    hexValues[hexPos] = (asciiValue - 97) + 10;
                }
                else
                {
                    throw new ArgumentException($"Invalid hex sign '{hexString[loop]}' at position {loop}!", nameof(hexString));
                }

                hexPos++;
                if (hexPos == 2)
                {
                    result[resultPos] = (byte)(hexValues[0] * 16 + hexValues[1]);
                    resultPos++;
                    hexPos = 0;
                }
            }

            return result;
        }
    }
}
