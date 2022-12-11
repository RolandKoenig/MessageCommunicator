using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace FirLib.Avalonia.PropertyGridControl;

public class PropertyGridEditControlFactory
{
    public virtual Control CreateControl(
        ConfigurablePropertyMetadata property, 
        string valuePropertyNameForBinding,
        IEnumerable<ConfigurablePropertyMetadata> allProperties)
    {
        Control? ctrlValueEdit = null;
        switch (property.ValueType)
        {
            case PropertyValueType.Bool:
                ctrlValueEdit = this.CreateCheckBoxControl(property, valuePropertyNameForBinding, allProperties);
                break;

            case PropertyValueType.String:
                ctrlValueEdit = this.CreateTextBoxControl(property, valuePropertyNameForBinding, allProperties);
                break;

            case PropertyValueType.Enum:
                ctrlValueEdit = this.CreateEnumControl(property, valuePropertyNameForBinding, allProperties);
                break;

            default:
                throw new ArgumentOutOfRangeException($"Unsupported value {property.ValueType}");
        }

        return ctrlValueEdit;
    }

    protected virtual Control CreateCheckBoxControl(
        ConfigurablePropertyMetadata property, 
        string valuePropertyNameForBinding,
        IEnumerable<ConfigurablePropertyMetadata> allProperties)
    {
        var ctrlCheckBox = new CheckBox();
        ctrlCheckBox[!ToggleButton.IsCheckedProperty] = new Binding(
            nameof(ConfigurablePropertyRuntime.ValueAccessor),
            BindingMode.TwoWay);
        ctrlCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
        ctrlCheckBox.IsEnabled = !property.IsReadOnly;
        return ctrlCheckBox;
    }

    protected virtual Control CreateTextBoxControl(
        ConfigurablePropertyMetadata property,
        string valuePropertyNameForBinding,
        IEnumerable<ConfigurablePropertyMetadata> allProperties)
    {
        var ctrlTextBox = new TextBox();
        ctrlTextBox[!TextBox.TextProperty] = new Binding(
            nameof(ConfigurablePropertyRuntime.ValueAccessor),
            BindingMode.TwoWay);
        ctrlTextBox.Width = double.NaN;
        ctrlTextBox.IsReadOnly = property.IsReadOnly;

        return ctrlTextBox;
    }

    protected virtual Control CreateEnumControl(
        ConfigurablePropertyMetadata property,
        string valuePropertyNameForBinding,
        IEnumerable<ConfigurablePropertyMetadata> allProperties)
    {
        var ctrlComboBox = new ComboBox();
        ctrlComboBox.Items = property.GetEnumMembers();
        ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
            nameof(ConfigurablePropertyRuntime.ValueAccessor),
            BindingMode.TwoWay);
        ctrlComboBox.Width = double.NaN;
        ctrlComboBox.IsEnabled = !property.IsReadOnly;
        return ctrlComboBox;
    }
}