using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class PropertyGridViewModel : OwnViewModelBase
    {
        private List<ConfigurablePropertyMetadata> _propertyMetadata;
        private object? _selectedObject;
        private IPropertyContractResolver? _propertyContractResolver;

        public object? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    this.RaisePropertyChanged(nameof(this.SelectedObject));

                    this.UpdatePropertyCollection();
                }
            }
        }

        public List<ConfigurablePropertyMetadata> PropertyMetadata
        {
            get => _propertyMetadata;
            private set
            {
                if (_propertyMetadata != value)
                {
                    _propertyMetadata = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public PropertyGridViewModel()
        {
            _propertyMetadata = new List<ConfigurablePropertyMetadata>(0);
        }

        public void SetPropertyContractResolver(IPropertyContractResolver? dataAnnotator)
        {
            _propertyContractResolver = dataAnnotator;
        }

        private void UpdatePropertyCollection()
        {
            var newPropertyMetadata = new List<ConfigurablePropertyMetadata>();

            var selectedObject = this.SelectedObject;
            if (selectedObject == null)
            {
                this.PropertyMetadata = newPropertyMetadata;
                return;
            }

            // Get properties for PropertyGrid
            PropertyDescriptorCollection properties;
            var metadataAttrib = selectedObject.GetType().GetCustomAttribute<MetadataTypeAttribute>();
            if (metadataAttrib != null)
            {
                properties = TypeDescriptor.GetProperties(metadataAttrib.MetadataClassType);
            }
            else
            {
                properties = TypeDescriptor.GetProperties(selectedObject);
            }

            // Create a viewmodel for each property
            foreach (PropertyDescriptor? actProperty in properties)
            {
                if (actProperty == null){ continue; }
                if (!actProperty.IsBrowsable){ continue; }

                var propMetadata = new ConfigurablePropertyMetadata(actProperty, selectedObject, _propertyContractResolver);
                if(propMetadata.ValueType == PropertyValueType.Unsupported){ continue; }

                newPropertyMetadata.Add(propMetadata);
            }

            this.PropertyMetadata = newPropertyMetadata;
        }
    }
}