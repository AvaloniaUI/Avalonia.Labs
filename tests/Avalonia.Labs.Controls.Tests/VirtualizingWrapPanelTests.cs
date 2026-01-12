using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Xunit;
using Avalonia.Themes.Fluent;

[assembly: AvaloniaTestApplication(typeof(Avalonia.Labs.Controls.Tests.VirtualizingWrapPanelTests))]

namespace Avalonia.Labs.Controls.Tests;

public class TestApp : Application
{
    public override void Initialize()
    {
        Avalonia.Themes.Fluent.FluentTheme fluentTheme = new();
        Styles.Add(fluentTheme);
    }
}

public class TestVirtualizingWrapPanel : VirtualizingWrapPanel
{
    public new Control? ScrollIntoView(int index) => base.ScrollIntoView(index);
    public new Control? ContainerFromIndex(int index) => base.ContainerFromIndex(index);
    public new IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap) => base.GetControl(direction, from, wrap);
    public new int IndexFromContainer(Control container) => base.IndexFromContainer(container);
    public int LastNavigationIndexPublic => base.LastNavigationIndex;
}

public class VirtualizingWrapPanelTests
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApp>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    [AvaloniaFact]
    public void Items_Are_Realized_When_In_Viewport()
    {
        var target = new TestVirtualizingWrapPanel
        {
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();
        
        // 100x100 viewport, 50x50 items -> 2x2 = 4 items should be realized
        Assert.Equal(0, target.FirstRealizedIndex);
        Assert.True(target.LastRealizedIndex >= 3);
    }

    [AvaloniaFact]
    public void ScrollIntoView_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            ItemSize = new Size(50, 50)
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 100).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        Assert.True(target.IsEffectivelyVisible, "Panel should be effectively visible");
        Assert.Equal(0, target.FirstRealizedIndex);

        // Scroll to item 50
        target.ScrollIntoView(50);
        window.UpdateLayout();

        // Verify that item 50 is realized
        Assert.True(target.FirstRealizedIndex <= 50, $"FirstRealizedIndex was {target.FirstRealizedIndex}");
        Assert.True(target.LastRealizedIndex >= 50, $"LastRealizedIndex was {target.LastRealizedIndex}");
        
        var container = target.ContainerFromIndex(50);
        Assert.NotNull(container);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        // Check if container is within viewport (approx)
        var transform = container.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container.Bounds.Size).TransformToAABB(transform.Value);
        
        Assert.True(rect.Top >= -1, $"rect.Top was {rect.Top}");
        Assert.True(rect.Bottom <= scrollViewer.Bounds.Height + 1, $"rect.Bottom was {rect.Bottom}, viewport height was {scrollViewer.Bounds.Height}");
    }

    [AvaloniaFact]
    public void AllowDifferentSizedItems_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        var items = new List<Size>
        {
            new Size(50, 50),
            new Size(60, 50),
            new Size(100, 50), // Should wrap to next row
            new Size(50, 60),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            Padding = new Thickness(0),
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height, Background = null }, true)
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();

        var container0 = target.ContainerFromIndex(0);
        var container1 = target.ContainerFromIndex(1);
        var container2 = target.ContainerFromIndex(2);
        
        Assert.NotNull(container0);
        Assert.NotNull(container1);
        Assert.NotNull(container2);

        Assert.Equal(0, container0.Bounds.Y);
        Assert.Equal(50, container1.Bounds.Y);
        Assert.Equal(100, container2.Bounds.Y);
    }

    [AvaloniaFact]
    public void Vertical_Orientation_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            Orientation = Orientation.Vertical,
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();

        // 100x100 viewport, 50x50 items -> 2x2 = 4 items should be realized
        Assert.Equal(0, target.FirstRealizedIndex);
        Assert.True(target.LastRealizedIndex >= 3);
        
        var container0 = target.ContainerFromIndex(0);
        var container1 = target.ContainerFromIndex(1);
        var container2 = target.ContainerFromIndex(2);
        
        Assert.NotNull(container0);
        Assert.NotNull(container1);
        Assert.NotNull(container2);

        // Vertical: item 0 at (0,0), item 1 at (0,50), item 2 at (50,0)
        Assert.Equal(0, container0.Bounds.X);
        Assert.Equal(0, container0.Bounds.Y);
        Assert.Equal(0, container1.Bounds.X);
        Assert.Equal(50, container1.Bounds.Y);
        Assert.Equal(50, container2.Bounds.X);
        Assert.Equal(0, container2.Bounds.Y);
    }

    private class TestItemSizeProvider : IItemSizeProvider
    {
        public Size GetSizeForItem(object item)
        {
            return (int)item % 2 == 0 ? new Size(50, 50) : new Size(60, 50);
        }
    }

    [AvaloniaFact]
    public void ItemSizeProvider_Works()
    {
        var provider = new TestItemSizeProvider();
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            ItemSizeProvider = provider,
            SpacingMode = SpacingMode.None
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).ToList(),
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();

        var container0 = target.ContainerFromIndex(0); // 50x50
        var container1 = target.ContainerFromIndex(1); // 60x50 -> should wrap
        
        Assert.NotNull(container0);
        Assert.NotNull(container1);

        Assert.Equal(0, container0.Bounds.Y);
        Assert.Equal(50, container1.Bounds.Y);
    }

    [AvaloniaFact]
    public void GetControl_Up_When_From_Is_Not_In_Panel_Does_Not_Crash()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            Orientation = Orientation.Horizontal,
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();

        var externalControl = new Button();

        // This should not throw IndexOutOfRangeException
        var result = target.GetControl(NavigationDirection.Up, externalControl, false);

        Assert.Null(result);
    }

    [AvaloniaFact]
    public void GetControl_Down_When_From_Is_Not_In_Panel_Does_Not_Crash()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            Orientation = Orientation.Horizontal,
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Content = itemsControl
        };

        window.Show();
        window.UpdateLayout();

        var externalControl = new Button();

        // This should not throw IndexOutOfRangeException
        var result = target.GetControl(NavigationDirection.Down, externalControl, false);

        Assert.Null(result);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_DifferentSizedItems_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Layout:
        // Row 0: [0: 50x50] [1: 40x50] -> Total Width 90
        // Row 1: [2: 30x50] [3: 60x50] -> Total Width 90
        // Row 2: [4: 100x50]           -> Wraps
        var items = new List<Size>
        {
            new Size(50, 50),
            new Size(40, 50),
            new Size(30, 50),
            new Size(60, 50),
            new Size(100, 50),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container0 = target.ContainerFromIndex(0);
        var container1 = target.ContainerFromIndex(1);
        var container2 = target.ContainerFromIndex(2);
        var container3 = target.ContainerFromIndex(3);
        var container4 = target.ContainerFromIndex(4);

        // Verify layout
        Assert.Equal(0, container0.Bounds.X); Assert.Equal(0, container0.Bounds.Y);
        Assert.Equal(50, container1.Bounds.X); Assert.Equal(0, container1.Bounds.Y);
        Assert.Equal(0, container2.Bounds.X); Assert.Equal(50, container2.Bounds.Y);
        Assert.Equal(30, container3.Bounds.X); Assert.Equal(50, container3.Bounds.Y);
        Assert.Equal(0, container4.Bounds.X); Assert.Equal(100, container4.Bounds.Y);

        // Down from 0 (x=0, mid=25) should go to 2 (x=0, mid=15) because it's closer than 3 (x=30, mid=60)
        var next = target.GetControl(NavigationDirection.Down, container0, false);
        Assert.Equal(container2, next);

        // Down from 1 (x=50, mid=70) should go to 3 (x=30, mid=60)
        next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(container3, next);

        // Up from 3 (x=30, mid=60) should go to 1 (x=50, mid=70)
        next = target.GetControl(NavigationDirection.Up, container3, false);
        Assert.Equal(container1, next);

        // Up from 2 (x=0, mid=15) should go to 0 (x=0, mid=25)
        next = target.GetControl(NavigationDirection.Up, container2, false);
        Assert.Equal(container0, next);

        // Down from 3 (x=30, mid=60) should go to 4 (x=0, mid=50)
        next = target.GetControl(NavigationDirection.Down, container3, false);
        Assert.Equal(container4, next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_DriftPrevention_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Layout:
        // Row 0: [0: 100x50] (mid 50)
        // Row 1: [1: 40x50] [2: 60x50] (mid 20, 70)
        // Row 2: [3: 100x50] (mid 50)
        var items = new List<Size>
        {
            new Size(100, 50),
            new Size(40, 50),
            new Size(60, 50),
            new Size(100, 50),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container0 = target.ContainerFromIndex(0);
        var container1 = target.ContainerFromIndex(1);
        var container2 = target.ContainerFromIndex(2);
        var container3 = target.ContainerFromIndex(3);

        // 1. Down from 0 (mid 50). 
        // Row 1 midpoints are 20 and 70. Both are equally far (30).
        // Overlap of 0 [0-100] with 1 [0-40] is 40. Overlap with 2 [40-100] is 60.
        // So it should pick 2.
        var next = target.GetControl(NavigationDirection.Down, container0, false);
        Assert.Equal(container2, next);

        // 2. Down again to Row 2.
        // If we drift to 2's midpoint (70), we'd use 70 for the next move.
        // But we should use the original anchor (50).
        // Row 2 has item 3 [0-100] (mid 50).
        // 50 is closer to 50 than 70 is (though 3 covers both).
        next = target.GetControl(NavigationDirection.Down, container2, false);
        Assert.Equal(container3, next);

        // 3. Up from 3 (using anchor 50).
        // Row 1 midpoints 20, 70. Both are dist 30 from 50.
        // Overlap of 3 [0-100] with 1 [0-40] is 40, with 2 [40-100] is 60.
        // Should pick 2.
        next = target.GetControl(NavigationDirection.Up, container3, false);
        Assert.Equal(container2, next);

        // 4. Up from 2 (using anchor 50).
        // Row 0 has item 0 [0-100] (mid 50).
        next = target.GetControl(NavigationDirection.Up, container2, false);
        Assert.Equal(container0, next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_EndRow_Down_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.Uniform,
            StretchItems = true
        };

        // Layout:
        // Width = 100.
        // Row 0: [0: 40x50] [1: 40x50]. Sum=80. 
        // With Stretch: extra = (100-80)/2 = 10. Items are 50x50.
        // Row 0: [0: 50x50 at X=0] [1: 50x50 at X=50]. Midpoints 25, 75.
        // Row 1: [2: 40x50]. Sum=40.
        // With Stretch: Item 2 becomes 100x50? Or 40+60=100?
        // Wait, GetRowLayout logic:
        // extraWidthPerItem = (rowWidth - summedUpChildWidth) / actualChildCount;
        // extra = (100-40)/1 = 60. Item 2 becomes 100x50. Midpoint 50.
        var items = new List<Size>
        {
            new Size(40, 50),
            new Size(40, 50),
            new Size(40, 50),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1); // Row 0, end (mid 75)
        var container2 = target.ContainerFromIndex(2); // Row 1, only item (mid 50)

        // Down from 1 (mid 75) should go to 2 (mid 50)
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(container2, next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_EndRow_To_ShorterRow_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Width 100
        // Row 0: [0: 50x50] [1: 50x50]. 1 ends at 100, mid 75.
        // Row 1: [2: 30x50]. Ends at 30, mid 15.
        var items = new List<Size>
        {
            new Size(50, 50),
            new Size(50, 50),
            new Size(30, 50),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1); // Row 0, end (mid 75, span 50-100)
        var container2 = target.ContainerFromIndex(2); // Row 1, only item (mid 15, span 0-30)

        // Down from 1 should go to 2
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(container2, next);
        
        // Add more items to Row 1 to see if it still catches the last one if we were at the end
        // Row 1: [2: 20x50] [3: 20x50] [4: 20x50] (mid 10, 30, 50). Total width 60.
        // If we are at Row 0 Item 1 (mid 75, span 50-100).
        // Item 4 (mid 50) is closest to 75.
        items = new List<Size>
        {
            new Size(50, 50),
            new Size(50, 50),
            new Size(20, 50),
            new Size(20, 50),
            new Size(20, 50),
        };
        itemsControl.ItemsSource = Enumerable.Range(0, items.Count).ToList();
        window.UpdateLayout();

        container1 = target.ContainerFromIndex(1);
        var container4 = target.ContainerFromIndex(4);
        
        next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(container4, next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_StretchAndSpacing_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.BetweenItemsOnly,
            StretchItems = true
        };

        // Width 100
        // Row 0: [0: 20x50] [1: 60x50]. Sum=80. 
        // StretchItems: extraWidthPerItem = (100-80)/2 = 10.
        // Row 0 Items:
        // 0: width 30, X=0, Mid=15
        // 1: width 70, X=30, Mid=65

        // Row 1: [2: 20x50] [3: 20x50] [4: 20x50]. Sum=60.
        // StretchItems: extraWidthPerItem = (100-60)/3 = 13.33.
        // Row 1 Items:
        // 2: width 33.33, X=0, Mid=16.66
        // 3: width 33.33, X=33.33, Mid=50
        // 4: width 33.33, X=66.66, Mid=83.33

        var items = new List<Size>
        {
            new Size(20, 50),
            new Size(60, 50),
            new Size(20, 50),
            new Size(20, 50),
            new Size(20, 50),
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1); // Row 0, item 1 (Mid 65)
        var container3 = target.ContainerFromIndex(3); // Row 1, item 3 (Mid 50) - Distance 15
        var container4 = target.ContainerFromIndex(4); // Row 1, item 4 (Mid 83.33) - Distance 18.33

        // Down from 1 should go to 3
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(container3, next);

        // From 3, go down again to no row below
        next = target.GetControl(NavigationDirection.Down, (IInputElement)container3, false);
        Assert.Equal(container3, next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_Down_WithAnchor_ShouldStayOnRightSide()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Width 100
        // Row 0: [0: 80x50] [1: 20x50]. X: 0, 80. Mid: 40, 90.
        // Row 1: [2: 100x50]. X: 0. Mid: 50.
        // Row 2: [3: 80x50] [4: 20x50]. X: 0, 80. Mid: 40, 90.
        
        var items = new List<Size>
        {
            new Size(80, 50), new Size(20, 50),
            new Size(100, 50),
            new Size(80, 50), new Size(20, 50)
        };

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 300,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1); // Row 0, index 1 (Mid 90)
        
        // 1. Down to Row 1 (Item 2). Anchor should be set to 90.
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.Equal(target.ContainerFromIndex(2), next);

        // 2. Down to Row 2. Should use anchor 90, so it should pick Item 4 (Mid 90).
        next = target.GetControl(NavigationDirection.Down, next, false);
        Assert.Equal(target.ContainerFromIndex(4), next);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_Down_ToOffscreenRow_ShouldStayOnRightSide()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Viewport height: 100
        // Each item: 50x50
        // Row 0: 0, 1 (mid 25, 75)
        // Row 1: 2, 3 (mid 25, 75)
        // Row 2: 4, 5 (mid 25, 75) - initially offscreen (Y=100)
        
        var items = Enumerable.Range(0, 20).Select(_ => new Size(50, 50)).ToList();

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            Padding = new Thickness(0),
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        // Start at item 1 (Row 0, Right side, Mid 75)
        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        // 1. Down to Row 1 (Item 3). Anchor should be set to 75.
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.NotNull(next);
        Assert.Equal(3, target.LastNavigationIndexPublic);

        // 2. Down to Row 2 (Item 5). This row was offscreen.
        next = target.GetControl(NavigationDirection.Down, (IInputElement)next!, false);
        Assert.NotNull(next);
        Assert.Equal(5, target.LastNavigationIndexPublic);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_Down_WithFractionalSizes_ShouldStayOnRightSide()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Viewport width: 100. Item width: 50.4.
        // Two items should NOT fit in one row (50.4 + 50.4 = 100.8 > 100).
        // Each item should be in its own row.
        // Row 0: item 0 (mid 25.2)
        // Row 1: item 1 (mid 25.2)
        // ...
        
        // HOWEVER, if CalculateAverageItemSize rounds 50.4 to 50.0.
        // Then for virtualized items, it will think 50 + 50 = 100 fits!
        // So it will think item 0 and 1 are in Row 0.
        
        var items = Enumerable.Range(0, 50).Select(_ => new Size(50.4, 50)).ToList();

        var itemsControl = new ItemsControl
        {
            Width = 100,
            Height = 100,
            Padding = new Thickness(0),
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        // Start at item 0 (Row 0)
        var container0 = target.ContainerFromIndex(0);
        Assert.NotNull(container0);
        
        // Down should go to item 1 (Row 1).
        var next = target.GetControl(NavigationDirection.Down, container0, false);
        Assert.NotNull(next);
        Assert.Equal(1, target.LastNavigationIndexPublic);

        // Down again should go to item 2 (Row 2).
        next = target.GetControl(NavigationDirection.Down, (IInputElement)next!, false);
        Assert.NotNull(next);
        Assert.Equal(2, target.LastNavigationIndexPublic);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_Down_CumulativeDrift_ShouldStayOnRightSide()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Viewport width: 1000. Item width: 333.4.
        // 3 items per row: 333.4 * 3 = 1000.2 > 1000. -> Only 2 items fit?
        // Wait, 1000.2 is very close to 1000. EPSILON is 0.00001. So 1000.2 > 1000.00001 is true.
        // So 2 items fit per row. Row 0: [0, 1]. Row 1: [2, 3].
        
        // If AverageItemSize rounds 333.4 to 333.0.
        // Then 333 * 3 = 999 <= 1000. -> It thinks 3 items fit!
        // Row 0: [0, 1, 2]. Row 1: [3, 4, 5].
        
        var items = Enumerable.Range(0, 100).Select(_ => new Size(333.4, 50)).ToList();

        var itemsControl = new ItemsControl
        {
            Width = 1000,
            Height = 200, // Show about 4 rows
            Padding = new Thickness(0),
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        // Start at item 1 (Row 0, Right side, Mid 166.7 + 333.4 = 500.1)
        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        // Down should go to item 3 (Row 1, Right side).
        // Actual Row 1 is [2, 3]. Item 3 mid: 333.4 + 166.7 = 500.1. Correct.
        
        // IF it thought 3 items per row:
        // Row 0: [0, 1, 2]. Item 1 is in the middle.
        // Row 1: [3, 4, 5]. Item 4 is in the middle.
        // It might pick item 4 instead of 3 if it thinks item 1 is at 500 and item 4 is at 500.
        
        var next = target.GetControl(NavigationDirection.Down, container1, false);
        Assert.NotNull(next);
        Assert.Equal(3, target.LastNavigationIndexPublic);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_RightEdge_Horizontal_Next_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            Orientation = Orientation.Horizontal,
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ListBox
        {
            Width = 100, // 2 items per row
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        // Item 1 is at the right edge of Row 0
        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        // NavigationDirection.Next from 1 should go to 2 (start of Row 1)
        var next = target.GetControl(NavigationDirection.Next, container1, false);
        Assert.NotNull(next);
        Assert.Equal(2, target.LastNavigationIndexPublic);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_RightEdge_Horizontal_Right_Works()
    {
        var target = new TestVirtualizingWrapPanel
        {
            Orientation = Orientation.Horizontal,
            ItemSize = new Size(50, 50)
        };

        var itemsControl = new ListBox
        {
            Width = 100, // 2 items per row
            Height = 200,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
        };

        var window = new Window { Content = itemsControl };
        window.Show();
        window.UpdateLayout();

        // Item 1 is at the right edge of Row 0
        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        // NavigationDirection.Right from 1 should go to 2 (start of Row 1)
        var next = target.GetControl(NavigationDirection.Right, container1, false);
        Assert.NotNull(next);
        Assert.Equal(2, target.LastNavigationIndexPublic);
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_Is_Visible()
    {
        var target = new TestVirtualizingWrapPanel
        {
            ItemSize = new Size(50, 50),
            SpacingMode = SpacingMode.None
        };

        var listBox = new ListBox
        {
            Width = 100, // Exactly 2 items per row
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 100).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Scroll to item 1 (right edge of first row)
        target.ScrollIntoView(1);
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container1.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container1.Bounds.Size).TransformToAABB(transform.Value);
        
        // Item 1 should be at X=50, Width=50. In a 100 width viewport, it should be fully visible.
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 1 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
        Assert.True(rect.Top >= -1 && rect.Bottom <= scrollViewer.Bounds.Height + 1, 
            $"Item 1 not vertically visible. Rect: {rect}, Viewport Height: {scrollViewer.Bounds.Height}");

        // Scroll to item 3 (right edge of second row)
        target.ScrollIntoView(3);
        window.UpdateLayout();

        var container3 = target.ContainerFromIndex(3);
        Assert.NotNull(container3);
        
        transform = container3.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        rect = new Rect(container3.Bounds.Size).TransformToAABB(transform.Value);

        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 3 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
        Assert.True(rect.Top >= -1 && rect.Bottom <= scrollViewer.Bounds.Height + 1, 
            $"Item 3 not vertically visible. Rect: {rect}, Viewport Height: {scrollViewer.Bounds.Height}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_Is_Visible_WithSpacing()
    {
        var target = new TestVirtualizingWrapPanel
        {
            ItemSize = new Size(40, 50),
            SpacingMode = SpacingMode.Uniform,
            StretchItems = true
        };

        var listBox = new ListBox
        {
            Width = 100, 
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, 100).Select(i => i.ToString()).ToList(),
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Row layout for 100 width, 2 items of 40 width:
        // extra = (100 - 80) / 2 = 10. Items become 50 width.
        // Item 0: X=0, W=50. Item 1: X=50, W=50.

        // Scroll to item 1
        target.ScrollIntoView(1);
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container1.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container1.Bounds.Size).TransformToAABB(transform.Value);
        
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 1 not horizontally visible with spacing. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_DifferentSizes_Is_Visible()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Row 0: 50, 40 (Total 90) -> Item 1 is at X=50, ends at 90.
        // Row 1: 100 (Total 100) -> Item 2 is at X=0, ends at 100.
        // Row 2: 30, 65 (Total 95) -> Item 4 is at X=30, ends at 95.
        var items = new List<Size>
        {
            new Size(50, 50), new Size(40, 50),
            new Size(100, 50),
            new Size(30, 50), new Size(65, 50),
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Scroll to item 4 (ends at 95, near right edge of 100)
        target.ScrollIntoView(4);
        window.UpdateLayout();

        var container4 = target.ContainerFromIndex(4);
        Assert.NotNull(container4);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container4.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container4.Bounds.Size).TransformToAABB(transform.Value);
        
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 4 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_Extreme_DifferentSizes_Is_Visible()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Row 0: 50, 49.9 (Total 99.9) -> Item 1 is at X=50, ends at 99.9.
        var items = new List<Size>
        {
            new Size(50, 50), new Size(49.9, 50),
            new Size(100, 50),
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Scroll to item 1 (ends at 99.9, extremely close to right edge of 100)
        target.ScrollIntoView(1);
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container1.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container1.Bounds.Size).TransformToAABB(transform.Value);
        
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 1 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_WithRoundingIssue_Is_Visible()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Item 0: 50.0001, Item 1: 49.9999. Total 100.0.
        // If EPSILON is 0.001, 50.0001 + 49.9999 = 100.0 <= 100 + 0.001.
        // They should be in the same row.
        var items = new List<Size>
        {
            new Size(50.0001, 50), new Size(49.9999, 50),
            new Size(100, 50),
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Scroll to item 1 (ends exactly at 100.0)
        target.ScrollIntoView(1);
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container1.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container1.Bounds.Size).TransformToAABB(transform.Value);
        
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 1 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_StretchedItem_Is_Fully_Visible()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            StretchItems = true,
            SpacingMode = SpacingMode.None
        };

        // Row 0: Item 0 (40x50), Item 1 (40x50). Total 80.
        // Stretched to 100: Item 0 (50x50), Item 1 (50x50).
        var items = new List<Size>
        {
            new Size(40, 50), new Size(40, 50),
            new Size(100, 50),
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Viewport width 100. 
        // We make the Viewport slightly smaller than the items row to force it to be "partially visible" if it was not stretched.
        // But here items are stretched to fill 100.
        
        // Actually, let's make the ListBox Width 90.
        // Items (40+40=80) stretched to 90 -> 45 each.
        // Item 1 ends at 90.
        listBox.Width = 90;
        window.UpdateLayout();

        // Scroll to item 1.
        target.ScrollIntoView(1);
        window.UpdateLayout();

        var container1 = target.ContainerFromIndex(1);
        Assert.NotNull(container1);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container1.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container1.Bounds.Size).TransformToAABB(transform.Value);
        
        // It should be fully visible [45, 90] in a [0, 90] viewport.
        Assert.True(rect.Left >= -1 && rect.Right <= scrollViewer.Bounds.Width + 1, 
            $"Item 1 not horizontally visible. Rect: {rect}, Viewport Width: {scrollViewer.Bounds.Width}");
    }

    [AvaloniaFact]
    public void ScrollIntoView_RightEdge_Item_Width_Greater_Than_Viewport()
    {
        var target = new TestVirtualizingWrapPanel
        {
            AllowDifferentSizedItems = true,
            SpacingMode = SpacingMode.None
        };

        // Item 0: 150x50. ListBox Width: 100.
        // Item 0 is wider than viewport.
        var items = new List<Size>
        {
            new Size(150, 50),
        };

        var listBox = new ListBox
        {
            Width = 100,
            Height = 100,
            ItemsPanel = new FuncTemplate<Panel?>(() => target),
            ItemsSource = Enumerable.Range(0, items.Count).ToList(),
            ItemTemplate = new FuncDataTemplate<int>((i, _) => new Canvas { Width = items[i].Width, Height = items[i].Height }, true)
        };

        var window = new Window
        {
            Width = 200,
            Height = 200,
            Content = listBox
        };

        window.Show();
        window.UpdateLayout();

        // Scroll to item 0.
        target.ScrollIntoView(0);
        window.UpdateLayout();

        var container0 = target.ContainerFromIndex(0);
        Assert.NotNull(container0);
        
        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        Assert.NotNull(scrollViewer);
        
        var transform = container0.TransformToVisual(scrollViewer);
        Assert.True(transform.HasValue);
        var rect = new Rect(container0.Bounds.Size).TransformToAABB(transform.Value);
        
        // When item is wider than viewport, BringIntoView typically aligns it to the left.
        Assert.True(rect.Left >= -1, 
            $"Item 0 left not visible. Rect: {rect}");
    }
}
