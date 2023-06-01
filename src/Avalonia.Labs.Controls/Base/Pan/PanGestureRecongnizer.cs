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
    private Point? _startPosition;
    private Point? _lastPosition;
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
        _visual = Target as Visual;
        _parent = _visual?.Parent as Visual;
        _state = PanGestureStatus.Completed;
        _startPosition = e.GetPosition(_parent);
        _state = PanGestureStatus.Started;
    }

    /// <inheritdoc />
    protected override void PointerMoved(PointerEventArgs e)
    {
        if (!_startPosition.HasValue)
        {
            return;
        }
        
        _lastPosition = e.GetPosition(_parent);
        _delta = _lastPosition.Value - _startPosition.Value;

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
            e.Pointer.Capture(_inputElement);
            OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0));
            e.Handled = true;
        }

        OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Running, _delta.X, _delta.Y));
        _state = PanGestureStatus.Running;
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        var startPosition = _startPosition;
        
        _startPosition = null;
        _lastPosition = null;
        
        if (!startPosition.HasValue || _state != PanGestureStatus.Running || e.Pointer.Captured != _inputElement)
        {
            return;
        }

        _state = PanGestureStatus.Completed;
        var currentPosition = e.GetPosition(_parent);
        var delta = currentPosition - startPosition.Value;
        OnPan?.Invoke(_inputElement,
            new PanUpdatedEventArgs(PanGestureStatus.Completed, delta.X, delta.Y));
        
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void PointerCaptureLost(IPointer pointer)
    {
        var startPosition = _startPosition;
        var delta = _delta;
        
        _startPosition = null;
        _lastPosition = null;
        _delta = default;
        
        if (!startPosition.HasValue ||_state != PanGestureStatus.Running)
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
