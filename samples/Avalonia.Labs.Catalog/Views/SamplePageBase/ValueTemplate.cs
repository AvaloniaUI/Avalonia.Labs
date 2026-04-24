using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace Avalonia.Labs.Catalog.Views.SamplePageBase;

public class ValueTemplate : IDataTemplate
{
    public Control Build(object? param)
    {
        var context = param as PropertyGridItem;
        var propertyType = context!.TargetProperty.PropertyType;

        if (propertyType == typeof(bool))
        {
            var control = new ToggleSwitch();
            CreateBinding(control, ToggleButton.IsCheckedProperty, context);
            return control;
        }

        if (propertyType.IsEnum)
        {
            var control = new ComboBox()
            {
                ItemsSource = Enum.GetValues(propertyType)
            };
            CreateBinding(control, SelectingItemsControl.SelectedItemProperty, context);
            return control;
        }

        
        return new TextBlock()
        {
            [!TextBlock.TextProperty] = new Binding(context.TargetProperty.Name)
            {
                Mode = BindingMode.OneWay,
                Source = context.TargetObject
            }
        };
    }

    private void CreateBinding(Control control, AvaloniaProperty controlProperty, PropertyGridItem context)
    {
        control[!controlProperty] = new Binding(context.TargetProperty.Name)
        {
            Mode = BindingMode.TwoWay,
            Source = context.TargetObject
        };
    }
    
    public bool Match(object? data)
    {
        return data is PropertyGridItem;
    }
}
