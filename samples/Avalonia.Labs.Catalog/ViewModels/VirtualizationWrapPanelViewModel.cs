using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Catalog.Views.SamplePageBase;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.Labs.Catalog.ViewModels;

public partial class VirtualizingWrapPanelViewModel : ViewModelBase, IItemSizeProvider
{
    static VirtualizingWrapPanelViewModel()
    {
        ViewLocator.Register(typeof(VirtualizingWrapPanelViewModel), () => new VirtualizingWrapPanelSampleView());
    }
    
    /// <summary>
    /// Gets some sample items
    /// </summary>
    public WrapPanelItemViewModel[] SampleItems { get; } = 
        Enumerable.Range(0, 1000)
            .Select(x => new WrapPanelItemViewModel($"Item {x:N0}"))
            .ToArray();

    [ObservableProperty]
    public partial WrapPanelItemViewModel? SelectedItem
    {
        get;
        set;
    }
    
    private bool _allowDifferentItemSizes;

    public bool AllowDifferentItemSizes
    {
        get => _allowDifferentItemSizes;
        set => this.SetProperty(ref _allowDifferentItemSizes, value);
    }

    [RelayCommand]
    private void RandomizeItemSizes()
    {
        AllowDifferentItemSizes = true;
        foreach (var item in SampleItems)
        {
            item.RandomizeItemSize();
        }
    }

    [RelayCommand]
    private void ResetItemSizes()
    {
        foreach (var item in SampleItems)
        {
            item.ResetItemSize();
        }
    }
    
    private Orientation _orientation = Orientation.Horizontal;

    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            this.SetProperty(ref _orientation, value);
        }
    }

    bool _gridLayout = true;

    public bool GridLayout
    {
        get => _gridLayout;
        set => this.SetProperty(ref _gridLayout, value);
    }
    
    SpacingMode _spacingMode;

    public SpacingMode SpacingMode
    {
        get => _spacingMode;
        set => this.SetProperty(ref _spacingMode, value);
    }
    
    public Size GetSizeForItem(object item)
    {
        if (item is WrapPanelItemViewModel itemVM)
        {
            return new Size(itemVM.ItemWidth, itemVM.ItemHeight);
        }
        throw new ArgumentException("Wrong item type");
    }
}

public class WrapPanelItemViewModel : ViewModelBase
{
    private static readonly Random Randomizer = new();
    
    /// <summary>
    /// Gets or sets the Item Width
    /// </summary>
    private double _ItemWidth = 128;

    public double ItemWidth
    {
        get => _ItemWidth;
        set => this.SetProperty(ref _ItemWidth, value);
    }
    
    /// <summary>
    /// Gets or sets the Item Height
    /// </summary>
    private double _ItemHeight = 128;

    public WrapPanelItemViewModel(string content)
    {
        Content = content;
    }

    public double ItemHeight
    {
        get => _ItemHeight;
        set => this.SetProperty(ref _ItemHeight, value);
    }

    /// <summary>
    /// Gets or sets the Content to show
    /// </summary>
    public string Content { get; }

    public void RandomizeItemSize()
    {
        ItemWidth = 50 + Randomizer.NextDouble() * 150; 
        ItemHeight = 50 + Randomizer.NextDouble() * 150;
    }
    
    public void ResetItemSize()
    {
        ItemWidth = 128; 
        ItemHeight = 128;
    }

    public override string ToString()
    {
        return Content;
    }
}
