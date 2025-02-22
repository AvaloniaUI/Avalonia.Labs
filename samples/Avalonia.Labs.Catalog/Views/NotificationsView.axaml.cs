using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Labs.Catalog.Views;

public partial class NotificationsView : UserControl
{
    private const double MOBILE_WIDTH_THRESHOLD = 600;
    private SplitView? _mainSplitView;
    private Button? _hamburgerButton;
    private bool _isNarrowLayout;

    public NotificationsView()
    {
        InitializeComponent();    
        _mainSplitView = this.FindControl<SplitView>("MainSplitView");
        _hamburgerButton = this.FindControl<Button>("HamburgerButton");
        if (_mainSplitView != null)
        {
            this.LayoutUpdated += OnLayoutUpdated;
        }
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (_mainSplitView == null || _hamburgerButton == null) return;

        var isNarrowNow = this.Bounds.Width <= MOBILE_WIDTH_THRESHOLD;
        if (_isNarrowLayout == isNarrowNow) return;

        _isNarrowLayout = isNarrowNow;

        if (_isNarrowLayout)
        {
            _mainSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            _mainSplitView.IsPaneOpen = false;
            _mainSplitView.CompactPaneLength = 0;
            _hamburgerButton.IsVisible = true;
        }
        else
        {
            _mainSplitView.DisplayMode = SplitViewDisplayMode.Inline;
            _mainSplitView.IsPaneOpen = true;
            _hamburgerButton.IsVisible = false;
        }
    }

    private void OnHamburgerButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_mainSplitView != null)
        {
            _mainSplitView.IsPaneOpen = !_mainSplitView.IsPaneOpen;
        }
    }
}
