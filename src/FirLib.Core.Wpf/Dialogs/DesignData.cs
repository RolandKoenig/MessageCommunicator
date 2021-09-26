using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FirLib.Core.Patterns.ErrorAnalysis;

namespace FirLib.Core.Dialogs
{
    internal static class DesignData
    {
        public static ErrorDialogViewModel? ExceptionInfo
        {
            get
            {
                try
                {
                    ThrowExceptionInternal();
                }
                catch (Exception e)
                {
                    return new ErrorDialogViewModel(new ExceptionInfo(e));
                }

                return null;
            }
        }

        private static void ThrowExceptionInternal()
        {
            throw new ApplicationException("Dummy Exception");
        }
    }
}
