using System;
using System.Collections.Generic;
using System.Text;
using FirLib.Core.Infrastructure;
using FirLib.Core.Services.ConfigurationFiles;
using FirLib.Core.Services.SingleApplicationInstance;

namespace FirLib.Core.Services
{
    public static class ServiceBootstrapExtensions
    {
        public static FirLibApplicationLoader AddConfigurationFileService(
            this FirLibApplicationLoader loader, 
            string appName)
        {
            loader.Services.Register(
                typeof(IConfigurationFileAccessors),
                new ConfigurationFileAccessors(appName));
            return loader;
        }

        public static FirLibApplicationLoader AddSingleApplicationInstanceService_UsingMutex(
            this FirLibApplicationLoader loader,
            string mutexName)
        { 
            loader.Services.Register(
                typeof(ISingleApplicationInstanceService),
                new MutexBasedSingleApplicationInstanceService(mutexName));
            return loader;
        }
    }
}
