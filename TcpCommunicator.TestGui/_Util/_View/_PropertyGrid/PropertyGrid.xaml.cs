using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    public class PropertyGrid : UserControl
    {
        public static readonly StyledProperty<object> SelectedObjectProperty =
            AvaloniaProperty.Register<PropertyGrid, object>(nameof(SelectedObject), typeof(object), notifying: OnSelectedObjectChanged);

        private PropertyGridViewModel _propertyGridVM;
        private Grid _gridMain;

        public object SelectedObject
        {
            get => this.GetValue(SelectedObjectProperty);
            set => this.SetValue(SelectedObjectProperty, value);
        }

        public PropertyGrid()
        {
            AvaloniaXamlLoader.Load(this);

            _gridMain = this.FindControl<Grid>("GridMain");

            _propertyGridVM = new PropertyGridViewModel();
            _gridMain.DataContext = _propertyGridVM;
        }

        private static void OnSelectedObjectChanged(IAvaloniaObject sender, bool beforeChanging)
        {
            if (beforeChanging) { return; }
            if (!(sender is PropertyGrid propGrid)) { return; }

            propGrid._propertyGridVM.SelectedObject = propGrid.SelectedObject;
            propGrid.UpdatePropertiesView();
        }

        private void UpdatePropertiesView()
        {
            _gridMain.Children.Clear();

            var lstProperties = new List<ConfigurablePropertyMetadata>(_propertyGridVM.PropertyMetadata);
            lstProperties.Sort((left, right) => left.CategoryName.CompareTo(right.CategoryName));
            var lstPropertyCategories = lstProperties
                .Select(actProperty => actProperty.CategoryName)
                .Distinct()
                .ToList();

            // Define rows
            _gridMain.RowDefinitions.Clear();
            var rowCount = lstProperties.Count + lstPropertyCategories.Count;
            for (var loop = 0; loop < rowCount; loop++)
            {
                _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35.0) });
            }

            // Create all controls
            var actRowIndex = 0;
            var actCategory = string.Empty;
            foreach (var actProperty in _propertyGridVM.PropertyMetadata)
            {
                if (actProperty.CategoryName != actCategory)
                {
                    actCategory = actProperty.CategoryName;

                    var txtHeader = new TextBlock
                    {
                        Text = actCategory
                    };

                    txtHeader.SetValue(Grid.RowProperty, actRowIndex);
                    txtHeader.SetValue(Grid.ColumnSpanProperty, 2);
                    txtHeader.SetValue(Grid.ColumnProperty, 0);
                    txtHeader.Margin = new Thickness(5d, 5d, 5d, 5d);
                    txtHeader.VerticalAlignment = VerticalAlignment.Bottom;
                    txtHeader.FontWeight = FontWeight.Bold;
                    _gridMain.Children.Add(txtHeader);

                    var rect = new Rectangle
                    {
                        Height = 2d,
                        Fill = new SolidColorBrush(Colors.Black),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(5d, 5d, 5d, 0d)
                    };

                    rect.SetValue(Grid.RowProperty, actRowIndex);
                    rect.SetValue(Grid.ColumnSpanProperty, 2);
                    rect.SetValue(Grid.ColumnProperty, 0);
                    _gridMain.Children.Add(rect);

                    actRowIndex++;
                }

                var ctrlText = new TextBlock
                {
                    Text = actProperty.PropertyDisplayName
                };

                ctrlText.SetValue(Grid.RowProperty, actRowIndex);
                ctrlText.SetValue(Grid.ColumnProperty, 0);
                ctrlText.Margin = new Thickness(5d, 5d, 50d, 5d);
                ctrlText.VerticalAlignment = VerticalAlignment.Center;
                _gridMain.Children.Add(ctrlText);

                Control? ctrlValueEdit = null;
                switch (actProperty.ValueType)
                {
                    case PropertyValueType.Bool:
                        var ctrlCheckBox = new CheckBox();
                        ctrlCheckBox[!ToggleButton.IsCheckedProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlValueEdit = ctrlCheckBox;
                        ctrlValueEdit.HorizontalAlignment = HorizontalAlignment.Left;
                        break;

                    case PropertyValueType.String:
                        var ctrlTextBox = new TextBox();
                        ctrlTextBox[!TextBox.TextProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlTextBox.Width = double.NaN;
                        ctrlValueEdit = ctrlTextBox;
                        break;

                    case PropertyValueType.Enum:
                        var ctrlComboBox = new ComboBox();
                        ctrlComboBox.Items = actProperty.GetEnumMembers();
                        ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlComboBox.Width = double.NaN;
                        ctrlValueEdit = ctrlComboBox;
                        break;

                    case PropertyValueType.DetailSettings:
                        break;
                }

                if (ctrlValueEdit != null)
                {
                    ctrlValueEdit.Margin = new Thickness(0d, 0d, 5d, 0d);
                    ctrlValueEdit.VerticalAlignment = VerticalAlignment.Center;
                    ctrlValueEdit.SetValue(Grid.RowProperty, actRowIndex);
                    ctrlValueEdit.SetValue(Grid.ColumnProperty, 1);
                    ctrlValueEdit.DataContext = actProperty;
                    _gridMain.Children.Add(ctrlValueEdit);
                }

                actRowIndex++;
            }
        }
    }
}
