using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirLib.Core.ViewServices;
using FirLib.Core.Wpf.Controls.CustomWindow;

namespace FirLib.Core.Patterns.Mvvm
{
    public class MvvmWindow : CustomWindowBase, IViewServiceHost
    {
        private ViewServiceContainer _viewServiceContainer;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServiceContainer.ViewServices;

        /// <inheritdoc />
        public IViewServiceHost? ParentViewServiceHost => null;

        public object? DialogResultMvvm { get; private set; }

        public MvvmWindow()
        {
            _viewServiceContainer = new ViewServiceContainer(this);

            this.DataContextChanged += this.OnDataContextChanged;
        }

        /// <inheritdoc />
        public virtual object? TryGetDefaultViewService(Type viewServiceType)
        {
            return WpfDefaultViewServices.TryGetViewService(this, viewServiceType);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is ViewModelBase oldViewModel)
            {
                if (ReferenceEquals(oldViewModel.AssociatedView, this))
                {
                    oldViewModel.CloseWindowRequest -= this.OnViewModel_CloseWindowRequest;  
                    oldViewModel.ViewServiceRequest -= this.OnViewModel_ViewServiceRequest;
                    oldViewModel.AssociatedView = null;
                }
            }

            if(e.NewValue is ViewModelBase newViewModel)
            {
                if (newViewModel.AssociatedView == null)
                {
                    newViewModel.CloseWindowRequest += this.OnViewModel_CloseWindowRequest;
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

        private void OnViewModel_CloseWindowRequest(object? sender, CloseWindowRequestEventArgs e)
        {
            this.DialogResultMvvm = e.DialogResult;
            this.Close();
        }
    }
}
