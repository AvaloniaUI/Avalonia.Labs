using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Avalonia.Labs.Catalog.Views.SamplePageBase;

public partial class ControlSampleBase : ContentControl
{
    public static readonly DirectProperty<ControlSampleBase, AvaloniaList<PropertyGridItem>> SamplePropertiesProperty = 
        AvaloniaProperty.RegisterDirect<ControlSampleBase, AvaloniaList<PropertyGridItem>>(
            nameof(SampleProperties), 
            c => c.SampleProperties );
    
    public AvaloniaList<PropertyGridItem> SampleProperties { get; } = new();

    public static readonly StyledProperty<object> SecondaryContentProperty =
        AvaloniaProperty.Register<ControlSampleBase, object>(nameof(SecondaryContent));

    public object SecondaryContent
    {
        get => GetValue(SecondaryContentProperty);
        set => SetValue(SecondaryContentProperty, value);
    }
    
    public ControlSampleBase()
    {
        
    }

    protected override Type StyleKeyOverride => typeof(ControlSampleBase);

    public ControlSampleBase(Visual content)
    {
        this.Content = content;
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        PreparePropertyGrid();
    }

    protected virtual void PreparePropertyGrid()
    {
        
    }
}

