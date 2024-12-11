using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Avalonia.Labs.Controls;

/// <summary>
/// These attached properties help with responsive layout:
/// if the screen width is smaller than MinimumDesktopWidth, then the controls horizontalalignment will be "Stretch", otherwise whatever was specified in "DesktopAlignment"
/// This allows stackpanels to be left- or right-aligned on desktops, but stretch horizontally on mobile devices
/// </summary>
public class ResponsiveHorizontalAlignment
{
    public static readonly AttachedProperty<HorizontalAlignment?> DesktopAlignmentProperty =
        AvaloniaProperty.RegisterAttached<Control, HorizontalAlignment?>(
            "DesktopAlignment", 
            typeof(ResponsiveHorizontalAlignment), 
            defaultValue: null);

    public static readonly AttachedProperty<int> MinimumDesktopWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, int>(
            "MinimumDesktopWidthProperty", 
            typeof(ResponsiveHorizontalAlignment), 
            defaultValue: 0);

    static ResponsiveHorizontalAlignment()
    {
        DesktopAlignmentProperty.Changed.AddClassHandler<Control>((x, e) => 
            OnDesktopAlignmentChanged(x, (HorizontalAlignment)e.NewValue!));
            
        MinimumDesktopWidthProperty.Changed.AddClassHandler<Control>((x, e) => 
            MinimumDesktopWidthChanged(x, (int)e.NewValue!));
    }

    public static void SetDesktopAlignment(Control element, HorizontalAlignment? value)
    {
        element.SetValue(DesktopAlignmentProperty, value);
    }

    public static HorizontalAlignment? GetDesktopAlignment(Control element)
    {
        return element.GetValue(DesktopAlignmentProperty);
    }

    public static void SetMinimumDesktopWidth(StackPanel element, bool value)
    {
        element.SetValue(MinimumDesktopWidthProperty, value);
    }

    public static int GetMinimumDesktopWidth(Control element)
    {
        return element.GetValue(MinimumDesktopWidthProperty);
    }

    private static void OnDesktopAlignmentChanged(Control panel, HorizontalAlignment? value)
    {
        if (value.HasValue)
        {
            SubscribeToTopLevel(panel);
            UpdateAlignment(panel);
        }
        else
        {
            UnsubscribeFromTopLevel(panel);
            panel.HorizontalAlignment = GetDesktopAlignment(panel) ?? default(HorizontalAlignment);
        }
    }

    private static void MinimumDesktopWidthChanged(Control panel, int value)
    {
        UpdateAlignment(panel);
    }

    private static readonly ConditionalWeakTable<Control, EventHandler> Subscriptions = new();

    private static void SubscribeToTopLevel(Control panel)
    {
        UnsubscribeFromTopLevel(panel); // Ensure no duplicate subscriptions
        
        var topLevel = Utilities.GetTopLevel(null);
        if (topLevel != null)
        {
            EventHandler handler = (s, e) => UpdateAlignment(panel);
            topLevel.LayoutUpdated += handler;

            if (Subscriptions.TryGetValue(panel, out var obsoleteHandler))
            {
                topLevel.LayoutUpdated -= obsoleteHandler;
                Subscriptions.Remove(panel);
            }
            Subscriptions.Add(panel, handler);
            
            UpdateAlignment(panel); // Initial update
        }
    }

    private static void UnsubscribeFromTopLevel(Control panel)
    {
        if (Subscriptions.TryGetValue(panel, out var handler))
        {
            var topLevel = Utilities.GetTopLevel(null);
            if (topLevel != null)
            {
                topLevel.LayoutUpdated -= handler;
            }
            Subscriptions.Remove(panel);
        }
    }

    private static void UpdateAlignment(Control panel)
    {
        var topLevel = Utilities.GetTopLevel(null);
        if (topLevel != null)
        {
            var screenWidth = topLevel.Bounds.Width;
            var desktopAlignment = GetDesktopAlignment(panel);
            var threshHoldWidth = GetMinimumDesktopWidth(panel);
            
            var isMobile = screenWidth < threshHoldWidth;
            
            panel.HorizontalAlignment = isMobile
                ? HorizontalAlignment.Stretch 
                : desktopAlignment??default(HorizontalAlignment);
            panel.MinWidth = isMobile ? 0 : threshHoldWidth;
        }
    }
}
