using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FirLib.Avalonia.PropertyGridControl.Mvvm;

namespace FirLib.Avalonia.PropertyGridControl;

internal class PropertyGridViewModel : PropertyChangedBase
{
    private List<ConfigurablePropertyRuntime> _propertyMetadata;
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

    public List<ConfigurablePropertyRuntime> PropertyMetadata
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
        _propertyMetadata = new List<ConfigurablePropertyRuntime>(0);
    }

    public void SetPropertyContractResolver(IPropertyContractResolver? dataAnnotator)
    {
        _propertyContractResolver = dataAnnotator;
    }

    private void UpdatePropertyCollection()
    {
        var newPropertyMetadata = new List<ConfigurablePropertyRuntime>();

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

            var propMetadata = new ConfigurablePropertyRuntime(actProperty, selectedObject, _propertyContractResolver);
            if(propMetadata.Metadata.ValueType == PropertyValueType.Unsupported){ continue; }

            newPropertyMetadata.Add(propMetadata);
        }

        this.PropertyMetadata = newPropertyMetadata;
    }
}