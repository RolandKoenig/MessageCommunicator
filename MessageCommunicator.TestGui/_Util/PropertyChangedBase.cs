using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MessageCommunicator.TestGui
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        private List<WeakPropertyChangedTarget>? _weakChangeTargets;

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var changeArgs = new PropertyChangedEventArgs(propertyName);

            // Trigger normal event targets
            this.PropertyChanged?.Invoke(this, changeArgs);

            // Trigger weak event targets
            if (_weakChangeTargets == null) { return; }
            for (var loop = 0; loop < _weakChangeTargets.Count; loop++)
            {
                var actEntry = _weakChangeTargets[loop];
                if(actEntry.Key.IsAlive)
                {
                    try { actEntry.Action(this, changeArgs); }
                    catch (Exception)
                    {
                        _weakChangeTargets.RemoveAt(loop);
                        throw;
                    }
                }
                else
                {
                    _weakChangeTargets.RemoveAt(loop);
                    loop--;
                }
            }
            if (_weakChangeTargets.Count == 0) { _weakChangeTargets = null; }
        }

        public void RegisterWeakPropertyChangedTarget(WeakReference key, Action<object, PropertyChangedEventArgs> action)
        {
            if(_weakChangeTargets == null){ _weakChangeTargets = new List<WeakPropertyChangedTarget>(4); }

            _weakChangeTargets.Add(new WeakPropertyChangedTarget(key, action));
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private readonly struct WeakPropertyChangedTarget
        {
            public readonly WeakReference Key;

            public readonly Action<object, PropertyChangedEventArgs> Action;

            public WeakPropertyChangedTarget(WeakReference key, Action<object, PropertyChangedEventArgs> action)
            {
                this.Key = key;
                this.Action = action;
            }
        }
    }
}
