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

        var topLevel = (TopLevel)e.Root;
        topLevel.RendererDiagnostics.DebugOverlays |= RendererDebugOverlays.Fps;
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        var topLevel = (TopLevel)e.Root;
        topLevel.RendererDiagnostics.DebugOverlays ^= RendererDebugOverlays.Fps;
    }
}

