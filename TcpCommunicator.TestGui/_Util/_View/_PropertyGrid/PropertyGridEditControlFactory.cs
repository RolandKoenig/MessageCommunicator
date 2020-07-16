using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace TcpCommunicator.TestGui
{
    public class PropertyGridEditControlFactory
    {
        public virtual Control? CreateControl(
            ConfigurablePropertyMetadata property, 
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            Control? ctrlValueEdit = null;
            switch (property.ValueType)
            {
                case PropertyValueType.Bool:
                    ctrlValueEdit = this.CreateCheckBoxControl(property, allProperties);
                    break;

                case PropertyValueType.String:
                    ctrlValueEdit = this.CreateTextBoxControl(property, allProperties);
                    break;

                case PropertyValueType.Enum:
                    ctrlValueEdit = this.CreateEnumControl(property, allProperties);
                    break;

                case PropertyValueType.EncodingWebName:
                    ctrlValueEdit = this.CreateEncodingWebNameControl(property, allProperties);
                    break;

                case PropertyValueType.TextAndHexadecimalEdit:
                    ctrlValueEdit = this.CreateTextAndHexadecimalEditControl(property, allProperties);
                    break;

                case PropertyValueType.DetailSettings:
                    break;
            }
            return ctrlValueEdit;
        }

        protected virtual Control CreateCheckBoxControl(
            ConfigurablePropertyMetadata property, 
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlCheckBox = new CheckBox();
            ctrlCheckBox[!ToggleButton.IsCheckedProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
            return ctrlCheckBox;
        }

        protected virtual Control CreateTextBoxControl(
            ConfigurablePropertyMetadata property, 
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlTextBox = new TextBox();
            ctrlTextBox[!TextBox.TextProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlTextBox.Width = double.NaN;
            return ctrlTextBox;
        }

        protected virtual Control CreateEnumControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var ctrlComboBox = new ComboBox();
            ctrlComboBox.Items = property.GetEnumMembers();
            ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlComboBox.Width = double.NaN;
            return ctrlComboBox;
        }

        protected virtual Control CreateEncodingWebNameControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;

            var ctrlComboBoxEnc = new ComboBox();
            ctrlComboBoxEnc.Items = Encoding.GetEncodings()
                .Select(actEncodingInfo => actEncodingInfo.Name);
            ctrlComboBoxEnc[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlComboBoxEnc.Width = double.NaN;

            var ctrlEncDescLabel = new TextBlock();
            ctrlEncDescLabel[!TextBlock.TextProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.OneWay)
            {
                Converter = new EncodingWebNameToDescriptionConverter()
            };

            stackPanel.Children.Add(ctrlComboBoxEnc);
            stackPanel.Children.Add(ctrlEncDescLabel);

            return stackPanel;
        }

        protected virtual Control CreateTextAndHexadecimalEditControl(
            ConfigurablePropertyMetadata property,
            IEnumerable<ConfigurablePropertyMetadata> allProperties)
        {
            var otherPropertyInfo = property.GetCustomAttribute<TextAndHexadecimalEditAttribute>();
            if (otherPropertyInfo == null)
            {
                throw new InvalidOperationException($"{nameof(TextAndHexadecimalEditAttribute)} not found on property {property.PropertyName}!");
            }

            var otherProperty = allProperties
                .FirstOrDefault(actProperty => actProperty.PropertyName == otherPropertyInfo.EncodingWebNamePropertyName);
            if (otherProperty == null)
            {
                throw new InvalidOperationException($"Property {otherPropertyInfo.EncodingWebNamePropertyName} not found!");
            }

            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;

            var ctrlTextBox1 = new TextBox();
            ctrlTextBox1[!TextBox.TextProperty] = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.TwoWay);
            ctrlTextBox1.Width = double.NaN;

            var hexTextBinding = new Binding(
                nameof(property.ValueAccessor),
                BindingMode.OneWay)
            {
                Converter = new TextToHexConverter(),
                ConverterParameter = new Func<string?>(() => otherProperty.ValueAccessor as string)
            };

            var ctrlTextBox2 = new TextBox();
            ctrlTextBox2[!TextBox.TextProperty] = hexTextBinding;
            ctrlTextBox2.Width = double.NaN;
            ctrlTextBox2.IsReadOnly = true;

            otherProperty.RegisterWeakPropertyChangedTarget(
                new WeakReference(ctrlTextBox2), 
                (sender, eArgs) =>
                {
                    ctrlTextBox2[!TextBox.TextProperty] = hexTextBinding;
                });

            stackPanel.Children.Add(ctrlTextBox1);
            stackPanel.Children.Add(ctrlTextBox2);

            return stackPanel;
        }
    }
}
