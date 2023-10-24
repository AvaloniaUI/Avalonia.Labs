using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Labs.Controls.Base.Pan;
using SkiaSharp;

namespace Avalonia.Labs.Catalog.Controls;

internal class PhotoCropperCanvas : BindableCanvas
{

    struct TouchPoint
    {
        public int CornerIndex { set; get; }
        public SKPoint Offset { set; get; }
    }

    private class TouchRecognizer : GestureRecognizer
    {
        private IInputElement? _inputElement;
        private Point? _startPosition;
        private Point? _lastPosition;
        private Point _delta;
        private PanGestureStatus _state;
        private Visual? _visual;
        private Visual? _parent;

        public EventHandler<TouchActionEventArgs>? Touch;


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
            Touch?.Invoke(_inputElement,
                new TouchActionEventArgs(e.Pointer.Id, TouchActionType.Pressed, _startPosition.Value, true));
            e.PreventGestureRecognition();
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
                Touch?.Invoke(_inputElement
                    , new(e.Pointer.Id, TouchActionType.Pressed, _lastPosition!.Value, true));
                e.Handled = true;
            }

            Touch?.Invoke(_inputElement,
                new(e.Pointer.Id, TouchActionType.Moved, _lastPosition!.Value, true));
            _state = PanGestureStatus.Running;
            e.Handled = true;
            e.PreventGestureRecognition();
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
            Touch?.Invoke(_inputElement,
                new(e.Pointer.Id, TouchActionType.Released, currentPosition, false));
            e.Handled = true;
            e.PreventGestureRecognition();
        }

        /// <inheritdoc />
        protected override void PointerCaptureLost(IPointer pointer)
        {
            var startPosition = _startPosition;
            var last = _startPosition;
            var delta = _delta;

            _startPosition = null;
            _lastPosition = null;
            _delta = default;

            if (!startPosition.HasValue || _state != PanGestureStatus.Running)
            {
                return;
            }

            Touch?.Invoke(_inputElement,
                new(pointer.Id,
                    TouchActionType.Cancelled,
                    _lastPosition!.Value,
                    false));
        }
    }

    const int CORNER = 50;      // pixel length of cropper corner
    const int RADIUS = 100;     // pixel radius of touch hit-test
    CroppingRectangle? croppingRect;
    SKMatrix inverseBitmapMatrix;
    Dictionary<long, TouchPoint> touchPoints = new Dictionary<long, TouchPoint>();
    readonly TouchRecognizer _touchRecognizer = new();
    bool _isAttached = false;

    // Drawing objects
    readonly SKPaint cornerStroke = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.White,
        StrokeWidth = 10
    };

    readonly SKPaint edgeStroke = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.White,
        StrokeWidth = 2
    };

    public PhotoCropperCanvas()
    {

    }

    public PhotoCropperCanvas(SKBitmap bitmap, float? aspectRatio = null)
    {
        this.Source = bitmap;

        SKRect bitmapRect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
        croppingRect = new CroppingRectangle(bitmapRect, aspectRatio);
    }


    public readonly static AvaloniaProperty<SKColor> CropperColorProperty =
        AvaloniaProperty.Register<PhotoCropperCanvas, SKColor>(nameof(CropperColor), SKColors.White);

    public SKColor CropperColor
    {
        get => this.GetValue<SKColor>(CropperColorProperty);
        set => this.SetValue(CropperColorProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (!_isAttached)
        {
            this.GestureRecognizers.Add(_touchRecognizer);
            _isAttached = true;
        }
        _touchRecognizer.Touch += OnTouchEffectTouchAction;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _touchRecognizer.Touch -= OnTouchEffectTouchAction;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SourceProperty && change.NewValue is SKBitmap bitmap)
        {
            SKRect bitmapRect = new(0, 0, bitmap.Width, bitmap.Height);
            croppingRect = new CroppingRectangle(bitmapRect, default);
        }
        if(change.Property == CropperColorProperty && change.NewValue is SKColor color)
        {
            edgeStroke.Color = color;
            cornerStroke.Color = color;
            InvalidateSurface();
        }
        base.OnPropertyChanged(change);
    }

    public override SKBitmap? Result
    {
        get
        {
            SKBitmap? croppedBitmap = default;

            if (Source is SKBitmap bitmap)
            {
                SKRect cropRect = croppingRect!.Rect;
                croppedBitmap = new SKBitmap((int)cropRect.Width,
                                                      (int)cropRect.Height);
                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = new SKRect(cropRect.Left, cropRect.Top,
                                           cropRect.Right, cropRect.Bottom);


                using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                {
                    canvas.DrawBitmap(bitmap, source, dest);
                }
            }
            return croppedBitmap;
        }
    }



    protected override void OnPaintSurface(SKImageInfo info, SKCanvas canvas, SKBitmap bitmap)
    {
        canvas.Clear(SKColors.Gray);

        // Calculate rectangle for displaying bitmap 
        float scale = Math.Min((float)info.Width / bitmap.Width, (float)info.Height / bitmap.Height);
        float x = (info.Width - scale * bitmap.Width) / 2;
        float y = (info.Height - scale * bitmap.Height) / 2;
        SKRect bitmapRect = new SKRect(x, y, x + scale * bitmap.Width, y + scale * bitmap.Height);
        canvas.DrawBitmap(bitmap, bitmapRect);

        // Calculate a matrix transform for displaying the cropping rectangle
        SKMatrix bitmapScaleMatrix = SKMatrix.CreateScaleTranslation(scale, scale, x, y);

        // Display rectangle
        SKRect scaledCropRect = bitmapScaleMatrix.MapRect(croppingRect.Rect);
        canvas.DrawRect(scaledCropRect, edgeStroke);

        // Display heavier corners
        using (SKPath path = new SKPath())
        {
            path.MoveTo(scaledCropRect.Left, scaledCropRect.Top + CORNER);
            path.LineTo(scaledCropRect.Left, scaledCropRect.Top);
            path.LineTo(scaledCropRect.Left + CORNER, scaledCropRect.Top);

            path.MoveTo(scaledCropRect.Right - CORNER, scaledCropRect.Top);
            path.LineTo(scaledCropRect.Right, scaledCropRect.Top);
            path.LineTo(scaledCropRect.Right, scaledCropRect.Top + CORNER);

            path.MoveTo(scaledCropRect.Right, scaledCropRect.Bottom - CORNER);
            path.LineTo(scaledCropRect.Right, scaledCropRect.Bottom);
            path.LineTo(scaledCropRect.Right - CORNER, scaledCropRect.Bottom);

            path.MoveTo(scaledCropRect.Left + CORNER, scaledCropRect.Bottom);
            path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom);
            path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom - CORNER);

            canvas.DrawPath(path, cornerStroke);
        }

        // Invert the transform for touch tracking
        bitmapScaleMatrix.TryInvert(out inverseBitmapMatrix);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
    }

    void OnTouchEffectTouchAction(object? sender, TouchActionEventArgs args)
    {
        if (IsVisible == false || croppingRect is null)
        {
            return;
        }
        SKPoint pixelLocation = ConvertToPixel(args.Location);
        SKPoint bitmapLocation = inverseBitmapMatrix.MapPoint(pixelLocation);

        switch (args.Type)
        {
            case TouchActionType.Pressed:
                // Convert radius to bitmap/cropping scale
                float radius = inverseBitmapMatrix.ScaleX * RADIUS;

                // Find corner that the finger is touching
                int cornerIndex = croppingRect.HitTest(bitmapLocation, radius);

                if (cornerIndex != -1 && !touchPoints.ContainsKey(args.Id))
                {
                    TouchPoint touchPoint = new TouchPoint
                    {
                        CornerIndex = cornerIndex,
                        Offset = bitmapLocation - croppingRect.Corners[cornerIndex]
                    };

                    touchPoints.Add(args.Id, touchPoint);
                }
                break;

            case TouchActionType.Moved:
                if (touchPoints.ContainsKey(args.Id))
                {
                    TouchPoint touchPoint = touchPoints[args.Id];
                    croppingRect.MoveCorner(touchPoint.CornerIndex,
                                            bitmapLocation - touchPoint.Offset);
                    base.InvalidateSurface();
                }
                break;

            case TouchActionType.Released:
            case TouchActionType.Cancelled:
                if (touchPoints.ContainsKey(args.Id))
                {
                    touchPoints.Remove(args.Id);
                }
                break;
        }
    }

    SKPoint ConvertToPixel(Point pt)
    {
        var bounds = this.Bounds;
        return new SKPoint((float)(CanvasSize.Width * pt.X / bounds.Width),
                            (float)(CanvasSize.Height * pt.Y / bounds.Height));
    }
}
