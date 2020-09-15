using System;
using System.Collections.Generic;

namespace MessageCommunicator.Util
{
    internal static class TcpCommunicatorUtil
    {
        public static void EnsureNoStartSymbolsInMessage(ReadOnlySpan<char> rawMessage, string startSymbols)
        {
            var startSymbolsMatch = false;
            var messageIndex = 0;
            for (var loop = 0; loop < startSymbols.Length; loop++)
            {
                if (startSymbols[loop] == rawMessage[messageIndex])
                {
                    messageIndex++;
                    startSymbolsMatch = true;
                }
                else
                {
                    startSymbolsMatch = false;
                    messageIndex = 0;
                }
            }

            if(startSymbolsMatch)
            {
                throw new ArgumentException($"StartSymbols found at index at the given message. Given message must not contain configured StartSymbols! ", nameof(rawMessage));
            }
        }

        public static void EnsureNoEndsymbolsInMessage(ReadOnlySpan<char> rawMessage, string endSymbols)
        {
            for (var loop = 0; loop < rawMessage.Length; loop++)
            {
                if (rawMessage[loop] == endSymbols[0])
                {
                    var endsymbolFound = true;
                    for (var loopEndSymbol = 1; (loopEndSymbol < endSymbols.Length) && 
                                                (loop + loopEndSymbol < rawMessage.Length); loopEndSymbol++)
                    {
                        if (rawMessage[loop + loopEndSymbol] != endSymbols[loopEndSymbol])
                        {
                            endsymbolFound = false;
                            break;
                        }
                    }
                    if (endsymbolFound)
                    {
                        throw new ArgumentException($"Endsymbols found at index {loop} of the given message. Given message must not contain configured endsymbols! ", nameof(rawMessage));
                    }
                }
            }
        }

        public static int ParseInt32FromStringPart(StringBuffer toParse, int index, int length)
        {
            var endIndex = index + length;
            var result = 0;
            for (var actIndex = index; actIndex < endIndex; actIndex++)
            {
                var actChar = toParse[actIndex];
                if((actChar < '0') || (actChar > '9'))
                {
                    throw new InvalidOperationException($"Unable to parse number out of string on index {index} with length {length}: {toParse}!");
                }

                result = result * 10 + (actChar - '0');
            }
            return result;
        }

        /// <summary>
        /// Gets the total count of digits for the given integer.
        /// </summary>
        public static int GetCountOfDigits(int n)
        {
            // Method taken from https://stackoverflow.com/questions/4483886/how-can-i-get-a-count-of-the-total-number-of-digits-in-a-number

            if (n >= 0)
            {
                if (n < 10) return 1;
                if (n < 100) return 2;
                if (n < 1000) return 3;
                if (n < 10000) return 4;
                if (n < 100000) return 5;
                if (n < 1000000) return 6;
                if (n < 10000000) return 7;
                if (n < 100000000) return 8;
                if (n < 1000000000) return 9;
                return 10;
            }
            else
            {
                if (n > -10) return 2;
                if (n > -100) return 3;
                if (n > -1000) return 4;
                if (n > -10000) return 5;
                if (n > -100000) return 6;
                if (n > -1000000) return 7;
                if (n > -10000000) return 8;
                if (n > -100000000) return 9;
                if (n > -1000000000) return 10;
                return 11;
            }
        }
    }
}
