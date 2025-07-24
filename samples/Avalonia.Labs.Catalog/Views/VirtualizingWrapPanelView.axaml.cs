﻿using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Labs.Catalog.ViewModels;
using Avalonia.Labs.Catalog.Views.SamplePageBase;
using Avalonia.Labs.Controls;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Catalog.Views;

public partial class VirtualizingWrapPanelView : UserControl
{
    public VirtualizingWrapPanelView()
    {
        InitializeComponent();
    }
    
    public VirtualizingWrapPanel? SamplePanel => this.GetVisualDescendants().OfType<VirtualizingWrapPanel>().FirstOrDefault();
}

public class VirtualizingWrapPanelSampleView() : ControlSampleBase(new VirtualizingWrapPanelView())
{
    private VirtualizingWrapPanelView Sample => Content as VirtualizingWrapPanelView ?? throw new InvalidOperationException();
    private VirtualizingWrapPanelViewModel ViewModel => (Sample.DataContext as VirtualizingWrapPanelViewModel)!;
    
    protected override void PreparePropertyGrid()
    {
        // Adding some sample properties to play with
        var panel = Sample.SamplePanel!;
        SampleProperties.Add(new PropertyGridItem(VirtualizingWrapPanel.SpacingModeProperty, panel));
        SampleProperties.Add(new PropertyGridItem(VirtualizingWrapPanel.AllowDifferentSizedItemsProperty, panel));
        SampleProperties.Add(new PropertyGridItem(VirtualizingWrapPanel.StretchItemsProperty, panel));
        SampleProperties.Add(new PropertyGridItem(VirtualizingWrapPanel.OrientationProperty, panel));
        SampleProperties.Add(new PropertyGridItem(VirtualizingWrapPanel.IsGridLayoutEnabledProperty, panel));

        // Adding some Buttons with commands
        var secondaryContent = new StackPanel();
        secondaryContent.Children.Add(new Button { Content = "Randomize Items", Command = ViewModel.RandomizeItemSizesCommand});
        secondaryContent.Children.Add(new Button { Content = "Reset Items", Command = ViewModel.ResetItemSizesCommand});
        SecondaryContent = secondaryContent;
    }
}
