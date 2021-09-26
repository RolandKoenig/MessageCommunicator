using System;
using System.Collections.Generic;
using System.Text;
using FirLib.Core.Infrastructure.Services;

namespace FirLib.Core.Infrastructure
{
    public class FirLibApplication
    {
        private static FirLibApplication? s_current;
        private static FirLibApplicationLoader? s_currentLoader;

        public static FirLibApplication Current
        {
            get
            {
                if (s_current == null)
                {
                    throw new InvalidOperationException($"{nameof(FirLibApplication)} is not initialized!");
                }
                return s_current;
            }
        }

        public static bool IsLoaded => s_current != null;

        private FirLibApplicationContext _context;

        public FirLibServiceContainer Services => _context.Services;

        internal static void Load(FirLibApplicationLoader loader)
        {
            if (s_current != null)
            {
                throw new FirLibException($"{nameof(FirLibApplication)} is already loaded!");
            }

            s_currentLoader = loader;
            s_current = new FirLibApplication(loader.GetContext());
        }

        internal static void Unload(FirLibApplicationLoader loader)
        {
            if (s_current == null)
            {
                throw new FirLibException($"{nameof(FirLibApplication)} is already unloaded!");
            }
            if (s_currentLoader != loader)
            {
                throw new FirLibException($"{nameof(FirLibApplication)} was loaded by another loader!");
            }

            s_current.UnloadInternal();
            s_current = null;
            s_currentLoader = null;
        }

        internal FirLibApplication(FirLibApplicationContext context)
        {
            _context = context;

            if(_context.LoadActions != null)
            {
                foreach(var actStartupAction in _context.LoadActions)
                {
                    actStartupAction();
                }
            }
        }

        public static FirLibApplicationLoader GetLoader()
        {
            return new FirLibApplicationLoader();
        }

        private void UnloadInternal()
        {
            // Call unload actions in reverse order
            var unloadActions = _context.UnloadActions;
            if (unloadActions is { Count: > 0 })
            {
                for (var loop = unloadActions.Count - 1; loop > -1; loop--)
                {
                    unloadActions[loop]();
                }
            }

            // Dispose registered services
            this.Services.Dispose();
        }
    }
}
