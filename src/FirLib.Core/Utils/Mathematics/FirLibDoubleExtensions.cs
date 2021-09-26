using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Utils.Mathematics
{
    public static class FirLibDoubleExtensions
    {
        private const double _3 = 0.001;
        private const double _4 = 0.0001;
        private const double _5 = 0.00001;
        private const double _6 = 0.000001;
        private const double _7 = 0.0000001;

        public static bool Equals3DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _3;
        }

        public static bool Equals4DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _4;
        }

        public static bool Equals5DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _5;
        }

        public static bool Equals6DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _6;
        }

        public static bool Equals7DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < _7;
        }
    }
}
