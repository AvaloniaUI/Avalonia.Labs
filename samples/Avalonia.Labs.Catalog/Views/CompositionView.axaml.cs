using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;

namespace Avalonia.Labs.Catalog.Views;

public partial class CompositionView : UserControl
{
    public CompositionView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (TopLevel.GetTopLevel(e.AttachmentPoint) is { } topLevel)
        {
            topLevel.RendererDiagnostics.DebugOverlays |= RendererDebugOverlays.Fps;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        if (TopLevel.GetTopLevel(e.Parent as Control) is { } topLevel)
        {
            topLevel.RendererDiagnostics.DebugOverlays ^= RendererDebugOverlays.Fps;
        }
    }
}

