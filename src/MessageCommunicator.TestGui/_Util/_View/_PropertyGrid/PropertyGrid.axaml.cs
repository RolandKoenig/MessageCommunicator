using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Text;
using Avalonia.Input;
using FirLib.Core.Patterns.Mvvm;

using MessageCommunicator.TestGui.ViewServices;

namespace MessageCommunicator.TestGui
{
    public class PropertyGrid : OwnUserControl<PropertyGridViewModel>
    {
        public static readonly StyledProperty<object?> SelectedObjectProperty =
            AvaloniaProperty.Register<PropertyGrid, object?>(
                nameof(SelectedObject), typeof(object), 
                notifying: OnSelectedObjectChanged);

        public static readonly StyledProperty<PropertyGridEditControlFactory?> EditControlFactoryProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyGridEditControlFactory?>(
                nameof(EditControlFactory),
                new PropertyGridEditControlFactory());

        public static readonly StyledProperty<IPropertyContractResolver?> PropertyContractResolverProperty =
            AvaloniaProperty.Register<PropertyGrid, IPropertyContractResolver?>(
                nameof(PropertyContractResolver));

        private PropertyGridViewModel _propertyGridVM;
        private Grid _gridMain;
        private Control? _firstValueRowEditor;

        public object? SelectedObject
        {
            get => this.GetValue(SelectedObjectProperty); 
            set => this.SetValue(SelectedObjectProperty, value);
        }

        public PropertyGridEditControlFactory? EditControlFactory
        {
            get => this.GetValue(EditControlFactoryProperty);
            set => this.SetValue(EditControlFactoryProperty, value);
        }

        public IPropertyContractResolver? PropertyContractResolver
        {
            get => this.GetValue(PropertyContractResolverProperty);
            set
            {
                this.SetValue(PropertyContractResolverProperty, value);
                _propertyGridVM.SetPropertyContractResolver(value);
            }
        }

        public PropertyGrid()
        {
            AvaloniaXamlLoader.Load(this);

            _gridMain = this.FindControl<Grid>("GridMain");

            _propertyGridVM = new PropertyGridViewModel();
            _gridMain.DataContext = _propertyGridVM;
        }

        public void FocusFirstValueRowEditor()
        {
            FocusManager.Instance.Focus(_firstValueRowEditor, NavigationMethod.Tab);
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
            _gridMain.RowDefinitions.Clear();

            var lstProperties = new List<ConfigurablePropertyMetadata>(_propertyGridVM.PropertyMetadata);
            lstProperties.Sort((left, right) => string.Compare(left.CategoryName, right.CategoryName, StringComparison.Ordinal));

            // Create all controls
            var actRowIndex = 0;
            var actCategory = string.Empty;
            var editControlFactory = this.EditControlFactory;
            if (editControlFactory == null){ editControlFactory = new PropertyGridEditControlFactory(); }

            foreach (var actProperty in _propertyGridVM.PropertyMetadata)
            {
                try
                {
                    // Create category rows
                    if (actProperty.CategoryName != actCategory)
                    {
                        _gridMain.RowDefinitions.Add(new RowDefinition {Height = new GridLength(35)});

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
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Margin = new Thickness(5d, 5d, 5d, 0d)
                        };
                        rect.Classes.Add("PropertyGridCategoryHeaderLine");

                        rect.SetValue(Grid.RowProperty, actRowIndex);
                        rect.SetValue(Grid.ColumnSpanProperty, 2);
                        rect.SetValue(Grid.ColumnProperty, 0);
                        _gridMain.Children.Add(rect);

                        actRowIndex++;
                    }

                    // Create row header
                    var ctrlTextContainer = new Border();
                    var ctrlText = new TextBlock();
                    ctrlText.Text = actProperty.PropertyDisplayName;
                    ctrlText.VerticalAlignment = VerticalAlignment.Center;
                    SetToolTip(ctrlText, actProperty.Description);
                    ctrlTextContainer.Height = 35.0;
                    ctrlTextContainer.Child = ctrlText;
                    ctrlTextContainer.SetValue(Grid.RowProperty, actRowIndex);
                    ctrlTextContainer.SetValue(Grid.ColumnProperty, 0);
                    ctrlTextContainer.VerticalAlignment = VerticalAlignment.Top;
                    _gridMain.Children.Add(ctrlTextContainer);

                    // Create and configure row editor
                    var ctrlValueEdit = editControlFactory.CreateControl(actProperty, _propertyGridVM.PropertyMetadata);
                    if (ctrlValueEdit != null)
                    {
                        _gridMain.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                        ctrlValueEdit.Margin = new Thickness(0d, 0d, 5d, 0d);
                        ctrlValueEdit.VerticalAlignment = VerticalAlignment.Center;
                        ctrlValueEdit.SetValue(Grid.RowProperty, actRowIndex);
                        ctrlValueEdit.SetValue(Grid.ColumnProperty, 1);
                        ctrlValueEdit.DataContext = actProperty;
                        SetToolTip(ctrlValueEdit, actProperty.Description);
                        _gridMain.Children.Add(ctrlValueEdit);

                        _firstValueRowEditor ??= ctrlValueEdit;

                        var helpLinkInfo = actProperty.GetCustomAttribute<HelpFileLinkAttribute>();
                        if (helpLinkInfo != null)
                        {
                            var helpButton = new Button();
                            helpButton.Content = "?";
                            helpButton.Width = 32;
                            helpButton.Height = 32;
                            helpButton.Margin = new Thickness(2);
                            helpButton.HorizontalAlignment = HorizontalAlignment.Center;
                            helpButton.VerticalAlignment = VerticalAlignment.Top;
                            helpButton.SetValue(Grid.RowProperty, actRowIndex);
                            helpButton.SetValue(Grid.ColumnProperty, 2);
                            SetToolTip(helpButton, actProperty.Description);
                            helpButton.Click += (sender, eArgs) =>
                            {
                                var srvHelp = this.TryFindViewService<IHelpViewerService>();
                                srvHelp?.ShowHelpPage(helpLinkInfo.HelpFileKey);
                            };
                            _gridMain.Children.Add(helpButton);
                        }
                    }
                    else
                    {
                        _gridMain.RowDefinitions.Add(new RowDefinition(1.0, GridUnitType.Pixel));
                    }

                    actRowIndex++;
                }
                catch (Exception e)
                {
                    CommonErrorHandling.Current.ShowErrorDialog(e);
                    break;
                }
            }
        }

        public static void SetToolTip(IAvaloniaObject targetObject, string toolTip)
        {
            if (string.IsNullOrEmpty(toolTip)) { return; }
            targetObject.SetValue(ToolTip.TipProperty, toolTip);
        }
    }
}
