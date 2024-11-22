using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels;

public class VirtualizingWrapPanelViewModel : ViewModelBase, IItemSizeProvider
{
    static VirtualizingWrapPanelViewModel()
    {
        ViewLocator.Register(typeof(VirtualizingWrapPanelViewModel), () => new VirtualizingWrapPanelView());
    }
    
    public VirtualizingWrapPanelViewModel()
    {
        RandomizeItemSizesCommand = ReactiveCommand.Create(RandomizeItemSizes);
        ResetItemSizesCommand = ReactiveCommand.Create(ResetItemSizes);
    }
    
    /// <summary>
    /// Gets some sample items
    /// </summary>
    public WrapPanelItemViewModel[] SampleItems { get; } = 
        Enumerable.Range(0, 1000)
            .Select(x => new WrapPanelItemViewModel($"Item {x:N0}"))
            .ToArray();

    private bool _allowDifferentItemSizes;

    public bool AllowDifferentItemSizes
    {
        get => _allowDifferentItemSizes;
        set => this.RaiseAndSetIfChanged(ref _allowDifferentItemSizes, value);
    }
    
    public ICommand RandomizeItemSizesCommand { get; }
    private void RandomizeItemSizes()
    {
        AllowDifferentItemSizes = true;
        foreach (var item in SampleItems)
        {
            item.RandomizeItemSize();
        }
    }

    public ICommand ResetItemSizesCommand { get; }
    private void ResetItemSizes()
    {
        foreach (var item in SampleItems)
        {
            item.ResetItemSize();
        }
    }

    public ScrollBarVisibility HorizontalScrollBarVisibility => Orientation == Orientation.Vertical ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
    public ScrollBarVisibility VerticalScrollBarVisibility => Orientation == Orientation.Vertical ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
    
    private Orientation _orientation = Orientation.Horizontal;

    public Orientation Orientation
    {
        get => _orientation;
        set
        {
            this.RaiseAndSetIfChanged(ref _orientation, value);
            this.RaisePropertyChanged(nameof(HorizontalScrollBarVisibility));
            this.RaisePropertyChanged(nameof(VerticalScrollBarVisibility));
        }
    }

    bool _gridLayout = true;

    public bool GridLayout
    {
        get => _gridLayout;
        set => this.RaiseAndSetIfChanged(ref _gridLayout, value);
    }
    
    SpacingMode _spacingMode;

    public SpacingMode SpacingMode
    {
        get => _spacingMode;
        set => this.RaiseAndSetIfChanged(ref _spacingMode, value);
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
        set => this.RaiseAndSetIfChanged(ref _ItemWidth, value);
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
        set => this.RaiseAndSetIfChanged(ref _ItemHeight, value);
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
}
