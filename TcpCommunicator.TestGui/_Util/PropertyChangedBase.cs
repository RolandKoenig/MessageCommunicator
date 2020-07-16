using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace TcpCommunicator.TestGui
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        private List<WeakPropertyChangedTarget>? _weakChangeTargets;

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var changeArgs = new PropertyChangedEventArgs(propertyName);
            this.PropertyChanged?.Invoke(this, changeArgs);

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
        }

        public void RegisterWeakPropertyChangedTarget(WeakReference key, Action<object, PropertyChangedEventArgs> action)
        {
            if(_weakChangeTargets == null){ _weakChangeTargets = new List<WeakPropertyChangedTarget>(4); }

            _weakChangeTargets.Add(new WeakPropertyChangedTarget(key, action));
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private struct WeakPropertyChangedTarget
        {
            public WeakReference Key { get; }

            public Action<object, PropertyChangedEventArgs> Action { get; }

            public WeakPropertyChangedTarget(WeakReference key, Action<object, PropertyChangedEventArgs> action)
            {
                this.Key = key;
                this.Action = action;
            }
        }
    }
}
