using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FirLib.Core.Infrastructure.Services;
using FirLib.Core.Patterns;

namespace FirLib.Core.Infrastructure
{
    public class FirLibApplicationLoader
    {
        private FirLibApplicationContext _context;

        public FirLibServiceContainer Services => _context.Services;

        internal FirLibApplicationLoader()
        {
            _context = new FirLibApplicationContext();
        }

        internal FirLibApplicationContext GetContext() => _context;

        public FirLibApplicationLoader AddLoadAction(Action action)
        {
            _context.LoadActions ??= new List<Action>();
            _context.LoadActions.Add(action);

            return this;
        }

        public FirLibApplicationLoader AddUnloadAction(Action action)
        {
            _context.UnloadActions ??= new List<Action>();
            _context.UnloadActions.Add(action);

            return this;
        }

        public FirLibApplicationLoader AddService(Type serviceType, object serviceSingleton)
        {
            this.Services.Register(
                serviceType, serviceSingleton);
            return this;
        }

        public FirLibApplicationLoader ConfigureCurrentThreadAsMainGuiThread()
        {
            this.AddLoadAction(() => Thread.CurrentThread.Name = FirLibConstants.MESSENGER_NAME_GUI);
            return this;
        }

        public IDisposable Load()
        {
            FirLibApplication.Load(this);

            return new DummyDisposable(
                () => FirLibApplication.Unload(this));
        }
    }
}
