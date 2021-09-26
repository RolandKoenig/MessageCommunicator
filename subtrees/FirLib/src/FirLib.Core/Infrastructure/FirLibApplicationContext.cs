using System;
using System.Collections.Generic;
using System.Text;
using FirLib.Core.Infrastructure.Services;

namespace FirLib.Core.Infrastructure
{
    internal class FirLibApplicationContext
    {
        public List<Action>? LoadActions { get; set; }

        public List<Action>? UnloadActions { get; set; }

        public FirLibServiceContainer Services { get; } = new();
    }
}
