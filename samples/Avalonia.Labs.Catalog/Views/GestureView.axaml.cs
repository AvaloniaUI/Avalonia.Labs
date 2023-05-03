using System;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Base;
using Avalonia.Labs.Controls.Base.Pan;
using Avalonia.Media.Transformation;

namespace Avalonia.Labs.Catalog.Views;

public partial class GestureView : UserControl
{
    private Point _startPosition;

    public GestureView()
    {
        InitializeComponent();
    }

    private void Pressed(object? sender, TapEventArgs e)
    {
        RedActionText.Content = $"Tap at {e.Position}";
    }

    private void Released(object? sender, TapEventArgs e)
    {
        BlueActionText.Content = $"Tap at {e.Position}";
    }

    private void Panned(object? sender, PanUpdatedEventArgs e)
    {
        WritePan(GreenActionText, Green, e);
    }

    private void WritePan(Label label, Border border, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case PanGestureStatus.Started:
                label.Content = $"S: {_startPosition.X} + {e.TotalX}, {_startPosition.Y} + {e.TotalY}";
                _startPosition = border.RenderTransform?.Value.GetPoint() ?? new Point(0, 0);
                break;
            case PanGestureStatus.Running:
                label.Content = $"R: {_startPosition.X} + {e.TotalX}, {_startPosition.Y} + {e.TotalY}";
                var transformOperation = TransformOperations.CreateBuilder(1);
                transformOperation.AppendTranslate(_startPosition.X + e.TotalX, _startPosition.Y + e.TotalY);

                border.SetValue(RenderTransformProperty, transformOperation.Build());
                break;
            case PanGestureStatus.Completed:
                label.Content = $"C: {_startPosition.X} + {e.TotalX}, {_startPosition.Y} + {e.TotalY}";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ReleasedOrange(object? sender, TapEventArgs e)
    {
        OrangeActionText.Content = $"Tap at {e.Position}";
    }

    private void PannedOrange(object? sender, PanUpdatedEventArgs e)
    {
        WritePan(OrangeActionText, Orange, e);
    }
}
