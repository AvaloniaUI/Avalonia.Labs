using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Labs.Catalog.Views;

public partial class MultiSelectionComboBoxView : UserControl
{
    public MultiSelectionComboBoxView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var mscb = this.Find<MultiSelectionComboBox>("MultiSelectionComboBox")!;
        var properties = this.Find<ItemsControl>("Properties");

        properties!.ItemsSource = new List<AvaloniaProperty>()
        {
            MultiSelectionComboBox.IsReadOnlyProperty,
            MultiSelectionComboBox.IsEditableProperty
        };

        properties.ItemTemplate = new FuncDataTemplate<AvaloniaProperty>(
            property => true,
            property =>
        {
            
            if (property.PropertyType == typeof(bool))
            {
                var checkBox = new CheckBox() {Content = property.Name};
                mscb.Bind(property, new Binding(nameof(checkBox.IsChecked), BindingMode.TwoWay));
                return checkBox;
            }

            return new TextBox(){Text = "Unknown DataType"};
        });
    }
}
