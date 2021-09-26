using System;
using System.Collections.Generic;
using System.Text;
using FirLib.Core.Infrastructure;
using FirLib.Core.Services.SingleApplicationInstance;

namespace FirLib.Core.Services
{
    public static class WinFormsServiceBootstrapExtensions
    {
        public static FirLibApplicationLoader AddSingleApplicationInstanceService_Using_WM_COPYDATA(
            this FirLibApplicationLoader loader,
            string controlName)
        { 
            loader.Services.Register(
                typeof(ISingleApplicationInstanceService),
                new WmCopyDataSingleApplicationInstanceService(controlName));
            return loader;
        }
    }
}
