using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FirLib.Core.ViewServices;

namespace FirLib.Core.Patterns.Mvvm
{
    public class MvvmUserControl : UserControl, IViewServiceHost
    {
        private ViewServiceContainer _viewServiceContainer;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServiceContainer.ViewServices;

        /// <inheritdoc />
        public IViewServiceHost? ParentViewServiceHost
        {
            get
            {
                var actParent = this.Parent as FrameworkElement;
                while (actParent != null)
                {
                    if (actParent is IViewServiceHost viewServiceHost) { return viewServiceHost; }
                    actParent = actParent.Parent as FrameworkElement;
                }
                return null;
            }
        }

        /// <inheritdoc />
        public virtual object? TryGetDefaultViewService(Type viewServiceType)
        {
            return WpfDefaultViewServices.TryGetViewService(this, viewServiceType);
        }

        public MvvmUserControl()
        {
            _viewServiceContainer = new ViewServiceContainer(this);

            this.DataContextChanged += this.OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is ViewModelBase oldViewModel)
            {
                if (ReferenceEquals(oldViewModel.AssociatedView, this))
                {
                    oldViewModel.ViewServiceRequest -= this.OnViewModel_ViewServiceRequest;
                    oldViewModel.AssociatedView = null;
                }
            }

            if(e.NewValue is ViewModelBase newViewModel)
            {
                if (newViewModel.AssociatedView == null)
                {
                    newViewModel.ViewServiceRequest += this.OnViewModel_ViewServiceRequest;
                    newViewModel.AssociatedView = this;
                }
            }
        }

        private void OnViewModel_ViewServiceRequest(object? sender, ViewServiceRequestEventArgs e)
        {
            var foundViewService = this.TryFindViewService(e.ViewServiceType);
            e.ViewService = foundViewService;
        }
    }
}
