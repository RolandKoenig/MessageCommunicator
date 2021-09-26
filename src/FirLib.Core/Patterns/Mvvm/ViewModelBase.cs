using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FirLib.Core.Patterns.Mvvm
{
    public class ViewModelBase : PropertyChangedBase
    {
        private object? _associatedView;

        public event EventHandler<CloseWindowRequestEventArgs>? CloseWindowRequest;

        public event EventHandler<ViewServiceRequestEventArgs>? ViewServiceRequest;

        [Browsable(false)]
        public object? AssociatedView
        {
            get => _associatedView;
            set
            {
                if(_associatedView != value)
                {
                    if (_associatedView != null)
                    {
                        this.OnMvvmViewDetaching();
                    }

                    _associatedView = value;

                    if(_associatedView != null)
                    {
                        this.OnMvvmViewAttached();
                    }
                }
            }
        }

        [Browsable(false)]
        public bool IsViewAttached => this.AssociatedView != null;

        protected void CloseWindow(object? dialogResult)
        {
            this.CloseWindowRequest?.Invoke(this, new CloseWindowRequestEventArgs(dialogResult));
        }

        /// <summary>
        /// Gets the view service of the given type.
        /// </summary>
        protected T GetViewService<T>()
            where T : class
        {
            var eventArgs = new ViewServiceRequestEventArgs(typeof(T));
            this.ViewServiceRequest?.Invoke(this, eventArgs);

            if (!(eventArgs.ViewService is T result))
            {
                throw new ApplicationException($"Unable to get view service of type {typeof(T).FullName}!");
            }
            return result;
        }

        /// <summary>
        /// Called when a mvvm view is attaching on this viewmodel.
        /// </summary>
        protected virtual void OnMvvmViewAttached()
        {

        }

        /// <summary>
        /// Called when a mvvm view is detaching from this viewmodel.
        /// </summary>
        protected virtual void OnMvvmViewDetaching()
        {

        }
    }
}
