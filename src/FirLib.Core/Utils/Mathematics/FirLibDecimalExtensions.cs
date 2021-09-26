using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Utils.Mathematics
{
    public static class FirLibDecimalExtensions
    {
        private const decimal _3 = 0.001m;
        private const decimal _4 = 0.0001m;
        private const decimal _5 = 0.00001m;
        private const decimal _6 = 0.000001m;
        private const decimal _7 = 0.0000001m;

        public static bool Equals3DigitPrecision(this decimal left, decimal right)
        {
            return Math.Abs(left - right) < _3;
        }

        public static bool Equals4DigitPrecision(this decimal left, decimal right)
        {
            return Math.Abs(left - right) < _4;
        }

        public static bool Equals5DigitPrecision(this decimal left, decimal right)
        {
            return Math.Abs(left - right) < _5;
        }

        public static bool Equals6DigitPrecision(this decimal left, decimal right)
        {
            return Math.Abs(left - right) < _6;
        }

        public static bool Equals7DigitPrecision(this decimal left, decimal right)
        {
            return Math.Abs(left - right) < _7;
        }
    }
}
