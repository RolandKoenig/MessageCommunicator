using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace FirLib.Avalonia.PropertyGridControl;

public class PropertyGrid : UserControl
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
        FocusManager.Instance?.Focus(_firstValueRowEditor, NavigationMethod.Tab);
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

        var lstProperties = new List<ConfigurablePropertyRuntime>(_propertyGridVM.PropertyMetadata);
        lstProperties.Sort((left, right) =>
            string.Compare(left.Metadata.CategoryName, right.Metadata.CategoryName, StringComparison.Ordinal));
        var allPropertiesMetadata = lstProperties.Select(x => x.Metadata);

        // Create all controls
        var actRowIndex = 0;
        var actCategory = string.Empty;
        var editControlFactory = this.EditControlFactory;
        if (editControlFactory == null) { editControlFactory = new PropertyGridEditControlFactory(); }

        foreach (var actProperty in _propertyGridVM.PropertyMetadata)
        {
            // Create category rows
            if (actProperty.Metadata.CategoryName != actCategory)
            {
                _gridMain.RowDefinitions.Add(new RowDefinition {Height = new GridLength(35)});

                actCategory = actProperty.Metadata.CategoryName;

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
            ctrlText.Text = actProperty.Metadata.PropertyDisplayName;
            ctrlText.VerticalAlignment = VerticalAlignment.Center;
            SetToolTip(ctrlText, actProperty.Metadata.Description);
            ctrlTextContainer.Height = 35.0;
            ctrlTextContainer.Child = ctrlText;
            ctrlTextContainer.SetValue(Grid.RowProperty, actRowIndex);
            ctrlTextContainer.SetValue(Grid.ColumnProperty, 0);
            ctrlTextContainer.VerticalAlignment = VerticalAlignment.Top;
            _gridMain.Children.Add(ctrlTextContainer);

            // Create and configure row editor
            var ctrlValueEdit = editControlFactory.CreateControl(actProperty.Metadata, nameof(actProperty.ValueAccessor), allPropertiesMetadata);
            if (ctrlValueEdit != null)
            {
                _gridMain.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                ctrlValueEdit.Margin = new Thickness(0d, 0d, 5d, 0d);
                ctrlValueEdit.VerticalAlignment = VerticalAlignment.Center;
                ctrlValueEdit.SetValue(Grid.RowProperty, actRowIndex);
                ctrlValueEdit.SetValue(Grid.ColumnProperty, 1);
                ctrlValueEdit.DataContext = actProperty;
                SetToolTip(ctrlValueEdit, actProperty.Metadata.Description);
                _gridMain.Children.Add(ctrlValueEdit);

                _firstValueRowEditor ??= ctrlValueEdit;
            }
            else
            {
                _gridMain.RowDefinitions.Add(new RowDefinition(1.0, GridUnitType.Pixel));
            }

            actRowIndex++;
        }
    }

    public static void SetToolTip(IAvaloniaObject targetObject, string toolTip)
    {
        if (string.IsNullOrEmpty(toolTip)) { return; }
        targetObject.SetValue(ToolTip.TipProperty, toolTip);
    }
}