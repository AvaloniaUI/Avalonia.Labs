using System;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;

namespace Avalonia.Labs.Controls.Base;

public enum DetectOn
{
    PointerPressed,
    PointerReleased
}

public class TapGestureRecognizer : GestureRecognizer
{
    private IInputElement? _inputElement;
    private Visual? _visual;
    private Visual? _parent;
    private Point? _startPosition;

    public float Threshold { get; set; } = 5;
    
    public DetectOn DetectOn { get; set; } = DetectOn.PointerReleased;
    
    public event EventHandler<TapEventArgs>? OnTap;

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        _inputElement = Target;
        _visual = Target as Visual;
        _parent = _visual?.Parent as Visual;
        _startPosition = e.GetPosition(_parent);
        if (DetectOn == DetectOn.PointerPressed)
        {
            OnTap?.Invoke(_inputElement, new TapEventArgs(_startPosition.Value));
            _startPosition = null;
            e.Handled = true;
        }
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (DetectOn != DetectOn.PointerReleased)
        {
            return;
        }
        
        if (!_startPosition.HasValue)
        {
            return;
        }
        
        var lastPosition = e.GetPosition(_parent);
        var delta = lastPosition - _startPosition.Value;
        if (Math.Abs(delta.X) < Threshold && Math.Abs(delta.Y) < Threshold)
        {
            OnTap?.Invoke(_inputElement, new TapEventArgs(lastPosition));
            e.Handled = true;
        }
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
    }

    protected override void PointerCaptureLost(IPointer pointer)
    {
        _startPosition = null;
    }
}
