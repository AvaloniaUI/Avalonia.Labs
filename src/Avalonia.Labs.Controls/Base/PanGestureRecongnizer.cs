using System;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;

namespace Avalonia.Labs.Controls.Base;

/// <summary>
/// Indicates pan gesture status
/// </summary>
public enum PanGestureStatus
{
    Started,
    Running,
    Completed
}

/// <summary>
/// Sets the pan directions
/// </summary>
[Flags]
public enum PanDirection
{
    /// <summary>
    /// Disables the pan
    /// </summary>
    None = 0,
    /// <summary>
    /// Allows pan to left
    /// </summary>
    Left = 1,
    /// <summary>
    /// Allows pan to right
    /// </summary>
    Right = 2,
    /// <summary>
    /// Allows pan up
    /// </summary>
    Up = 4,
    /// <summary>
    /// Allows pan down
    /// </summary>
    Down = 8,
}

/// <summary>
/// Contains the pan updates event data 
/// </summary>
public class PanUpdatedEventArgs : EventArgs
{
    public PanUpdatedEventArgs(PanGestureStatus statusType, double totalX, double totalY)
    {
        StatusType = statusType;
        TotalX = totalX;
        TotalY = totalY;
    }

    public PanGestureStatus StatusType { get; set; }
    public double TotalX { get; set; }
    public double TotalY { get; set; }
}

/// <summary>
/// The gesture recognizer for pan gesture 
/// </summary>
public class PanGestureRecognizer : IGestureRecognizer
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
    public void Initialize(IInputElement target, IGestureRecognizerActionsDispatcher actions)
    {
        _inputElement = target;
        _visual = target as Visual;
        _parent = _visual?.Parent as Visual;
        _state = PanGestureStatus.Completed;
    }

    /// <inheritdoc />
    public void PointerPressed(PointerPressedEventArgs e)
    {
        _startPosition = e.GetPosition(_parent);
        _state = PanGestureStatus.Started;
        OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0));
    }

    /// <inheritdoc />
    public void PointerMoved(PointerEventArgs e)
    {
        if (!_startPosition.HasValue)
        {
            return;
        }

        _state = PanGestureStatus.Running;
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

        if (e.Pointer.Captured == null)
        {
            e.Pointer.Capture(_inputElement);
        }

        if ((Math.Abs(_delta.X) > Threshold || Math.Abs(_delta.Y) > Threshold) && e.Pointer.Captured == null)
        {
            _startPosition = null;
            _lastPosition = null;
            return;
        }

        OnPan?.Invoke(_inputElement, new PanUpdatedEventArgs(PanGestureStatus.Running, _delta.X, _delta.Y));
    }

    /// <inheritdoc />
    public void PointerReleased(PointerReleasedEventArgs e)
    {
        if (!_startPosition.HasValue || _state == PanGestureStatus.Completed)
        {
            return;
        }

        _state = PanGestureStatus.Completed;
        var currentPosition = e.GetPosition(_parent);
        var delta = currentPosition - _startPosition.Value;
        OnPan?.Invoke(_inputElement,
            new PanUpdatedEventArgs(PanGestureStatus.Completed, delta.X, delta.Y));

        _startPosition = null;
        _lastPosition = null;
    }

    /// <inheritdoc />
    public void PointerCaptureLost(IPointer pointer)
    {
        if (!_startPosition.HasValue || _state == PanGestureStatus.Completed)
        {
            return;
        }

        OnPan?.Invoke(_inputElement,
            new PanUpdatedEventArgs(
                PanGestureStatus.Completed,
                _delta.X,
                _delta.Y));

        _startPosition = null;
        _lastPosition = null;
        _delta = default;
    }
}
