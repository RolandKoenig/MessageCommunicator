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
using SharpDX.Text;
using Encoding = System.Text.Encoding;

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
            //var rowCount = lstProperties.Count + lstPropertyCategories.Count;
            //for (var loop = 0; loop < rowCount; loop++)
            //{
            //    _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70.0) });
            //}

            // Create all controls
            var actRowIndex = 0;
            var actCategory = string.Empty;
            foreach (var actProperty in _propertyGridVM.PropertyMetadata)
            {
                if (actProperty.CategoryName != actCategory)
                {
                    _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });

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
                        Height = 1d,
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

                var ctrlText = new TextBlock();
                ctrlText.Text = actProperty.PropertyDisplayName;
                ctrlText.VerticalAlignment = VerticalAlignment.Center;

                var ctrlTextContainer = new Border();
                ctrlTextContainer.Height = 35.0;
                ctrlTextContainer.Child = ctrlText;
                ctrlTextContainer.SetValue(Grid.RowProperty, actRowIndex);
                ctrlTextContainer.SetValue(Grid.ColumnProperty, 0);
                ctrlTextContainer.VerticalAlignment = VerticalAlignment.Top;
                _gridMain.Children.Add(ctrlTextContainer);

                Control? ctrlValueEdit = null;
                switch (actProperty.ValueType)
                {
                    case PropertyValueType.Bool:
                        _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
                        var ctrlCheckBox = new CheckBox();
                        ctrlCheckBox[!ToggleButton.IsCheckedProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlValueEdit = ctrlCheckBox;
                        ctrlValueEdit.HorizontalAlignment = HorizontalAlignment.Left;
                        break;

                    case PropertyValueType.String:
                        _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
                        var ctrlTextBox = new TextBox();
                        ctrlTextBox[!TextBox.TextProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlTextBox.Width = double.NaN;
                        ctrlValueEdit = ctrlTextBox;
                        break;

                    case PropertyValueType.Enum:
                        _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
                        var ctrlComboBox = new ComboBox();
                        ctrlComboBox.Items = actProperty.GetEnumMembers();
                        ctrlComboBox[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlComboBox.Width = double.NaN;
                        ctrlValueEdit = ctrlComboBox;
                        break;

                    case PropertyValueType.EncodingWebName:
                        _gridMain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(75) });
                        var stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Vertical;

                        var ctrlComboBoxEnc = new ComboBox();
                        ctrlComboBoxEnc.Items = Encoding.GetEncodings()
                            .Select(actEncodingInfo => actEncodingInfo.Name);
                        ctrlComboBoxEnc[!SelectingItemsControl.SelectedItemProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.TwoWay);
                        ctrlComboBoxEnc.Width = double.NaN;

                        var ctrlEncDescLabel = new TextBlock();
                        ctrlEncDescLabel[!TextBlock.TextProperty] = new Binding(
                            nameof(actProperty.ValueAccessor),
                            BindingMode.OneWay)
                        {
                            Converter = new EncodingWebNameToDescriptionConverter()
                        };

                        stackPanel.Children.Add(ctrlComboBoxEnc);
                        stackPanel.Children.Add(ctrlEncDescLabel);

                        ctrlValueEdit = stackPanel;
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
