using System;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Reactive;

namespace Avalonia.Labs.Controls;

/// <summary>
/// Special control to host a <see cref="ContentDialog"/>/>
/// </summary>
/// <remarks>
/// This class should generally not be used outside of FluentAvalonia, and is
/// only public for Xaml styling support
/// </remarks>
public class DialogHost : ContentControl
{
    public DialogHost()
    {
        Background = null;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
    }

    protected override Type StyleKeyOverride => typeof(OverlayPopupHost);

    protected override Size MeasureOverride(Size availableSize)
    {
        _ = base.MeasureOverride(availableSize);

        if (VisualRoot is TopLevel tl)
        {
            return tl.ClientSize;
        }
        else if (VisualRoot is Control c)
        {
            return c.Bounds.Size;
        }

        return default;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (e.Root is Control wb)
        {
            // OverlayLayer is a Canvas, so we won't get a signal to resize if the window
            // bounds change. Subscribe to force update

            var observer = new AnonymousObserver<Rect>(_ => InvalidateMeasure());

            _rootBoundsWatcher = wb.GetObservable(BoundsProperty)
                .Subscribe(observer);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _rootBoundsWatcher?.Dispose();
        _rootBoundsWatcher = null;
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        e.Handled = true;
    }

    private IDisposable? _rootBoundsWatcher;
}
