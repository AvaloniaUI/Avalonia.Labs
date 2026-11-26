using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

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
            var control = new ComboBox();
            if (propertyType == typeof(SpacingMode))
                control.ItemsSource = Enum.GetValues<SpacingMode>();
            else if (propertyType == typeof(Orientation))
                control.ItemsSource = Enum.GetValues<Orientation>();
            else
                throw new NotImplementedException();
            CreateBinding(control, SelectingItemsControl.SelectedItemProperty, context);
            return control;
        }

        return new TextBlock()
        {
            [!TextBlock.TextProperty] = new CompiledBinding(new CompiledBindingPathBuilder()
                .Property(context.TargetProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build())
            {
                Mode = BindingMode.OneWay,
                Source = context.TargetObject
            }
        };
    }

    private void CreateBinding(Control control, AvaloniaProperty controlProperty, PropertyGridItem context)
    {
        control[!controlProperty] = new CompiledBinding(new CompiledBindingPathBuilder()
            .Property(context.TargetProperty, PropertyInfoAccessorFactory.CreateAvaloniaPropertyAccessor).Build())
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
