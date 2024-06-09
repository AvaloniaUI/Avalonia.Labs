using System;

using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;

namespace Avalonia.Labs.Controls.Base.Pan;

/// <summary>
/// The gesture recognizer for pan gesture 
/// </summary>
public class PanGestureRecognizer : GestureRecognizer
{
    private IInputElement? _inputElement;
    private IPointer? _tracking;
    private Point _startPosition;
    private Point _delta;
    private PanGestureStatus _state;
    private Visual? _visual;
    private Visual? _parent;

    public event EventHandler<PanUpdatedEventArgs>? OnPan;

    public PanDirection Direction { get; set; } =
        PanDirection.Left | PanDirection.Right | PanDirection.Up | PanDirection.Down;

    public float Threshold { get; set; } = 5;

    /// <inheritdoc />
    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        _inputElement = Target;
        _tracking = e.Pointer;
        _visual = Target as Visual;
        _parent = _visual?.Parent as Visual;
        _startPosition = e.GetPosition(_parent);
        _state = PanGestureStatus.Started;
    }

    /// <inheritdoc />
    protected override void PointerMoved(PointerEventArgs e)
    {
        if (e.Pointer != _tracking)
        {
            return;
        }

        var currentPosition = e.GetPosition(_parent);
        _delta = currentPosition - _startPosition;

        var currentDirection = PanDirection.None;
        if (_delta.X < -Threshold)
        {
            currentDirection |= PanDirection.Left;
        }
        else if (_delta.X > Threshold)
        {
            currentDirection |= PanDirection.Right;
        }

        if (_delta.Y < -Threshold)
        {
            currentDirection |= PanDirection.Up;
        }
        else if (_delta.Y > Threshold)
        {
            currentDirection |= PanDirection.Down;
        }

        if ((currentDirection & Direction) == 0)
        {
            return;
        }

        if (Math.Abs(_delta.X) < Threshold && Math.Abs(_delta.Y) < Threshold)
        {
            return;
        }

        if (_state == PanGestureStatus.Started)
        {
            OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0));
            e.Handled = true;
            Capture(e.Pointer);
        }

        OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Running, _delta.X, _delta.Y));
        _state = PanGestureStatus.Running;
        e.Handled = true;
        e.PreventGestureRecognition();
    }

    /// <inheritdoc />
    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Pointer != _tracking)
        {
            return;
        }

        _tracking = null;

        if (_state != PanGestureStatus.Running)
        {
            return;
        }

        _state = PanGestureStatus.Completed;
        var currentPosition = e.GetPosition(_parent);
        var delta = currentPosition - _startPosition;
        OnPan?.Invoke(_inputElement,
            new PanUpdatedEventArgs(PanGestureStatus.Completed, delta.X, delta.Y));

        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void PointerCaptureLost(IPointer pointer)
    {
        var delta = _delta;

        _tracking = null;
        _delta = default;

        if (_state != PanGestureStatus.Running)
        {
            return;
        }

        OnPan?.Invoke(_inputElement,
            new PanUpdatedEventArgs(
                PanGestureStatus.Completed,
                delta.X,
                delta.Y));
    }
}
