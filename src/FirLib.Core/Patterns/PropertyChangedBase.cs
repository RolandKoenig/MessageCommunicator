using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FirLib.Core.Patterns
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        private List<WeakPropertyChangedTarget>? _weakChangeTargets;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        //*** Code from project PRISM (https://github.com/PrismLibrary/Prism)
        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            this.RaisePropertyChanged(propertyName);

            return true;
        }

        //*** Code from project PRISM (https://github.com/PrismLibrary/Prism)
        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged.Invoke();
            this.RaisePropertyChanged(propertyName);

            return true;
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = "")
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
            _weakChangeTargets ??= new List<WeakPropertyChangedTarget>(4);

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
