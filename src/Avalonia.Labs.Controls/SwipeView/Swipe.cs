using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Base.Pan;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;

// <summary>
/// A control that provides swipe gestures to reveal items behind the main content.
/// Supports swiping in all four directions.
/// </summary>
public class Swipe : Grid
{
    // Pixels to move before locking axis (horizontal vs vertical).
    private const int DirectionLockThreshold = 5;

    // Pixels before the pan gesture actually starts.
    private const int GestureThreshold = 10;
    
    // Default threshold in pixels.
    private const double DefaultThreshold = 100.0;

    /// <summary>
    /// Defines the <see cref="Threshold"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ThresholdProperty =
        AvaloniaProperty.Register<Swipe, double>(nameof(Threshold), DefaultThreshold);

    /// <summary>
    /// Gets or sets the number of device-independent units that trigger a swipe gesture to fully reveal swipe items
    /// or execute them in Execute mode. Default is 100.
    /// </summary>
    public double Threshold
    {
        get => GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<Swipe, TimeSpan>(nameof(AnimationDuration), TimeSpan.FromMilliseconds(200));

    /// <summary>
    /// Gets or sets the duration of the open/close snap animation.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }


    /// <summary>
    /// Defines the <see cref="Right"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> RightTemplateProperty =
        AvaloniaProperty.Register<Swipe, IDataTemplate?>(nameof(Right));

    /// <summary>
    /// Gets or sets the content to be displayed when swiping from right to left.
    /// </summary>
    public IDataTemplate? Right
    {
        get => GetValue(RightTemplateProperty);
        set => SetValue(RightTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Left"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> LeftTemplateProperty =
        AvaloniaProperty.Register<Swipe, IDataTemplate?>(nameof(Left));

    /// <summary>
    /// Gets or sets the content to be displayed when swiping from left to right.
    /// </summary>
    public IDataTemplate? Left
    {
        get => GetValue(LeftTemplateProperty);
        set => SetValue(LeftTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Top"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> TopTemplateProperty =
        AvaloniaProperty.Register<Swipe, IDataTemplate?>(nameof(Top));

    /// <summary>
    /// Gets or sets the content to be displayed when swiping from top to bottom.
    /// </summary>
    public IDataTemplate? Top
    {
        get => GetValue(TopTemplateProperty);
        set => SetValue(TopTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Bottom"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> BottomTemplateProperty =
        AvaloniaProperty.Register<Swipe, IDataTemplate?>(nameof(Bottom));

    /// <summary>
    /// Gets or sets the content to be displayed when swiping from bottom to top.
    /// </summary>
    public IDataTemplate? Bottom
    {
        get => GetValue(BottomTemplateProperty);
        set => SetValue(BottomTemplateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Content"/> property.
    /// </summary>
    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<Swipe, Control?>(nameof(Content));

    /// <summary>
    /// Gets or sets the primary content of the Swipe component that covers the swipe items.
    /// </summary>
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SwipeState"/> property.
    /// </summary>
    public static readonly StyledProperty<SwipeState> SwipeStateProperty =
        AvaloniaProperty.Register<Swipe, SwipeState>(nameof(SwipeState));

    /// <summary>
    /// Gets or sets the current open state of the Swipe component.
    /// </summary>
    public SwipeState SwipeState
    {
        get => GetValue(SwipeStateProperty);
        set => SetValue(SwipeStateProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="LeftMode"/> property.
    /// </summary>
    public static readonly StyledProperty<SwipeMode> LeftModeProperty =
        AvaloniaProperty.Register<Swipe, SwipeMode>(nameof(LeftMode), SwipeMode.Reveal);

    /// <summary>
    /// Gets or sets the swipe mode for the Left items.
    /// </summary>
    public SwipeMode LeftMode
    {
        get => GetValue(LeftModeProperty);
        set => SetValue(LeftModeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="RightMode"/> property.
    /// </summary>
    public static readonly StyledProperty<SwipeMode> RightModeProperty =
        AvaloniaProperty.Register<Swipe, SwipeMode>(nameof(RightMode), SwipeMode.Reveal);

    /// <summary>
    /// Gets or sets the swipe mode for the Right items.
    /// </summary>
    public SwipeMode RightMode
    {
        get => GetValue(RightModeProperty);
        set => SetValue(RightModeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="TopMode"/> property.
    /// </summary>
    public static readonly StyledProperty<SwipeMode> TopModeProperty =
        AvaloniaProperty.Register<Swipe, SwipeMode>(nameof(TopMode), SwipeMode.Reveal);

    /// <summary>
    /// Gets or sets the swipe mode for the Top items.
    /// </summary>
    public SwipeMode TopMode
    {
        get => GetValue(TopModeProperty);
        set => SetValue(TopModeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="BottomMode"/> property.
    /// </summary>
    public static readonly StyledProperty<SwipeMode> BottomModeProperty =
        AvaloniaProperty.Register<Swipe, SwipeMode>(nameof(BottomMode), SwipeMode.Reveal);

    /// <summary>
    /// Gets or sets the swipe mode for the Bottom items.
    /// </summary>
    public SwipeMode BottomMode
    {
        get => GetValue(BottomModeProperty);
        set => SetValue(BottomModeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="OpenRequested"/> event.
    /// </summary>
    public static readonly RoutedEvent<OpenRequestedEventArgs> OpenRequestedEvent =
        RoutedEvent.Register<Swipe, OpenRequestedEventArgs>(nameof(OpenRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the swipe control is requested to open via a gesture or method call.
    /// </summary>
    public event EventHandler<OpenRequestedEventArgs>? OpenRequested
    {
        add => AddHandler(OpenRequestedEvent, value);
        remove => RemoveHandler(OpenRequestedEvent, value);
    }

    /// <summary>
    /// Defines the <see cref="CloseRequested"/> event.
    /// </summary>
    public static readonly RoutedEvent<CloseRequestedEventArgs> CloseRequestedEvent =
        RoutedEvent.Register<Swipe, CloseRequestedEventArgs>(nameof(CloseRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the swipe control is requested to close.
    /// </summary>
    public event EventHandler<CloseRequestedEventArgs>? CloseRequested
    {
        add => AddHandler(CloseRequestedEvent, value);
        remove => RemoveHandler(CloseRequestedEvent, value);
    }

    /// <summary>
    /// Defines the <see cref="SwipeStarted"/> event.
    /// </summary>
    public static readonly RoutedEvent<SwipeStartedEventArgs> SwipeStartedEvent =
        RoutedEvent.Register<Swipe, SwipeStartedEventArgs>(nameof(SwipeStarted), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a swipe gesture begins and the direction has been locked.
    /// </summary>
    public event EventHandler<SwipeStartedEventArgs>? SwipeStarted
    {
        add => AddHandler(SwipeStartedEvent, value);
        remove => RemoveHandler(SwipeStartedEvent, value);
    }

    /// <summary>
    /// Occurs as the swipe moves.
    /// </summary>
    public event EventHandler<SwipeChangingEventArgs>? SwipeChanging;

    /// <summary>
    /// Defines the <see cref="SwipeEnded"/> event.
    /// </summary>
    public static readonly RoutedEvent<SwipeEndedEventArgs> SwipeEndedEvent =
        RoutedEvent.Register<Swipe, SwipeEndedEventArgs>(nameof(SwipeEnded), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a swipe gesture completes, indicating if it resulted in an open state.
    /// </summary>
    public event EventHandler<SwipeEndedEventArgs>? SwipeEnded
    {
        add => AddHandler(SwipeEndedEvent, value);
        remove => RemoveHandler(SwipeEndedEvent, value);
    }

    private readonly ContentPresenter _rightContainer;
    private readonly ContentPresenter _leftContainer;
    private readonly ContentPresenter _topContainer;
    private readonly ContentPresenter _bottomContainer;
    private readonly ContentPresenter _bodyContainer;
    private readonly TransformOperationsTransition _transition;
    private readonly PanGestureRecognizer _panGestureRecognizer;
    
    // Cache this object to avoid allocating it on every frame during a swipe.
    private readonly SwipeChangingEventArgs _cachedSwipeChangingArgs;
    internal event EventHandler<SwipeDirection>? ExecuteRequested;

    private double _initialX;
    private double _currentX;
    private double _initialY;
    private double _currentY;
    
    private bool _isHorizontalSwipe;
    private bool _isVerticalSwipe;
    private SwipeDirection _swipeDirection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Swipe"/> class.
    /// </summary>
    public Swipe()
    {
        ClipToBounds = true;
        Focusable = true;

        _rightContainer = CreateSideContainer(HorizontalAlignment.Right, VerticalAlignment.Stretch);
        _leftContainer = CreateSideContainer(HorizontalAlignment.Left, VerticalAlignment.Stretch);
        _topContainer = CreateSideContainer(HorizontalAlignment.Stretch, VerticalAlignment.Top);
        _bottomContainer = CreateSideContainer(HorizontalAlignment.Stretch, VerticalAlignment.Bottom);

        _bodyContainer = new ContentPresenter
        {
            Transitions = new Transitions()
        };

        _transition = new TransformOperationsTransition
        {
            Property = RenderTransformProperty,
            Duration = AnimationDuration, 
            Easing = new CubicEaseOut()
        };

        _bodyContainer.Transitions ??= new Transitions();
        _bodyContainer.Transitions.Add(_transition);

        _panGestureRecognizer = new PanGestureRecognizer
        {
            Direction = PanDirection.Left | PanDirection.Right | PanDirection.Up | PanDirection.Down,
            Threshold = GestureThreshold,
        };

        _panGestureRecognizer.OnPan += PanUpdated;
        _bodyContainer.GestureRecognizers.Add(_panGestureRecognizer);
        
        _cachedSwipeChangingArgs = new SwipeChangingEventArgs(SwipeDirection.Left, 0);

        Children.Add(_rightContainer);
        Children.Add(_leftContainer);
        Children.Add(_topContainer);
        Children.Add(_bottomContainer);
        Children.Add(_bodyContainer);
    }

    /// <summary>
    /// Handle keyboard shortcuts for accessibility (Ctrl+Arrows to open, Esc to close).
    /// </summary>
    /// <param name="e">Key event args.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled)
            return;

        var ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);

        if (ctrl)
        {
            switch (e.Key)
            {
                case Key.Left:
                    SetSwipeState(SwipeState.LeftVisible);
                    e.Handled = true;
                    return;
                case Key.Right:
                    SetSwipeState(SwipeState.RightVisible);
                    e.Handled = true;
                    return;
                case Key.Up:
                    SetSwipeState(SwipeState.TopVisible);
                    e.Handled = true;
                    return;
                case Key.Down:
                    SetSwipeState(SwipeState.BottomVisible);
                    e.Handled = true;
                    return;
            }
        }

        if (e.Key == Key.Escape)
        {
            SetSwipeState(SwipeState.Hidden);
            e.Handled = true;
        }
    }

    private ContentPresenter CreateSideContainer(HorizontalAlignment hAlign, VerticalAlignment vAlign)
    {
        var presenter = new ContentPresenter
        {
            IsVisible = false,
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };

        // For top/bottom containers, ensure they don't stretch vertically beyond content
        if (vAlign == VerticalAlignment.Top || vAlign == VerticalAlignment.Bottom)
        {
            presenter.VerticalAlignment = vAlign;
            // ClipToBounds ensures the container doesn't overflow
            presenter.ClipToBounds = true;
        }

        return presenter;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == ContentProperty)
        {
            _bodyContainer.Content = e.NewValue;
        }
        else if (e.Property == SwipeStateProperty)
        {
            ProcessSwipe(SwipeState);
        }
        else if (e.Property == AnimationDurationProperty)
        {
            _transition.Duration = e.GetNewValue<TimeSpan>();
        }
    }

    private SwipeState CalculateState(double translationX, double translationY)
    {
        var absX = Math.Abs(translationX);
        var absY = Math.Abs(translationY);
        var configuredThreshold = Threshold > 0 ? Threshold : DefaultThreshold;

        if (absX > absY)
        {
            var sideContainer = translationX < 0 ? _rightContainer : _leftContainer;
            double containerWidth = Math.Max(sideContainer.Bounds.Width, GetContainerExtent(true, sideContainer));
        
            if (containerWidth <= 0) 
                return SwipeState.Hidden;
            
            var effectiveThreshold = Math.Min(configuredThreshold, containerWidth);

            if (absX >= effectiveThreshold)
            {
                return translationX < 0 ? SwipeState.RightVisible : SwipeState.LeftVisible;
            }

            return SwipeState.Hidden;
        }
        else
        {
            var sideContainer = translationY < 0 ? _bottomContainer : _topContainer;
            double containerHeight = Math.Max(sideContainer.Bounds.Height, GetContainerExtent(false, sideContainer));
       
            if (containerHeight <= 0)
                return SwipeState.Hidden;

            var effectiveThreshold = Math.Min(configuredThreshold, containerHeight);

            if (absY >= effectiveThreshold)
            {
                return translationY < 0 ? SwipeState.BottomVisible : SwipeState.TopVisible;
            }

            return SwipeState.Hidden;
        }
    }

    private void ProcessSwipe(SwipeState state)
    {
        switch (state)
        {
            case SwipeState.RightVisible:
                _rightContainer.IsVisible = true;
                MaterializeDataTemplate(_rightContainer, Right);
                ApplySwipeTransform(() => SetTranslate(-_rightContainer.Bounds.Width, 0), _rightContainer);
                FocusFirstFocusable(_rightContainer);
                break;

            case SwipeState.LeftVisible:
                _leftContainer.IsVisible = true;
                MaterializeDataTemplate(_leftContainer, Left);
                ApplySwipeTransform(() => SetTranslate(_leftContainer.Bounds.Width, 0), _leftContainer);
                FocusFirstFocusable(_leftContainer);
                break;

            case SwipeState.TopVisible:
                _topContainer.IsVisible = true;
                MaterializeDataTemplate(_topContainer, Top);
                ApplySwipeTransform(() => SetTranslate(0, _topContainer.Bounds.Height), _topContainer);
                FocusFirstFocusable(_topContainer);
                break;

            case SwipeState.BottomVisible:
                _bottomContainer.IsVisible = true;
                MaterializeDataTemplate(_bottomContainer, Bottom);
                ApplySwipeTransform(() => SetTranslate(0, -_bottomContainer.Bounds.Height), _bottomContainer);
                FocusFirstFocusable(_bottomContainer);
                break;

            case SwipeState.Hidden:
            default:
                SetTranslate(0, 0);
                // Return focus to the main body when closed.
                _bodyContainer.Focus();
                break;
        }
    }

    private void FocusFirstFocusable(ContentPresenter presenter)
    {
        if (presenter.Content is Control control && control.Focusable)
        {
            control.Focus();
            return;
        }

        if (presenter.Content is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Control c && c.Focusable)
                {
                    c.Focus();
                    return;
                }
            }
        }
    }

    private double GetContainerExtent(bool horizontal, ContentPresenter container)
    {
        if (horizontal)
        {
            return Math.Max(container.Bounds.Width, container.DesiredSize.Width);
        }

        return Math.Max(container.Bounds.Height, container.DesiredSize.Height);
    }

    private void ApplySwipeTransform(Action transformAction, ContentPresenter container)
    {
        if (container.Bounds.Width > 0 || container.Bounds.Height > 0)
        {
            transformAction();
        }
        else
        {
            // If bounds are not ready, defer to next render pass.
            Dispatcher.UIThread.Post(transformAction, DispatcherPriority.Render);
        }

    }

    private void SetTranslate(double x, double y)
    {
        _currentX = x;
        _currentY = y;

        var transformOperation = TransformOperations.CreateBuilder(1);
        transformOperation.AppendTranslate(x, y);

        _bodyContainer.SetValue(RenderTransformProperty, transformOperation.Build());

        bool isRight = x < 0;
        bool isLeft = x > 0;
        bool isTop = y > 0;
        bool isBottom = y < 0;

        if (_rightContainer.IsVisible != isRight) _rightContainer.IsVisible = isRight;
        if (_leftContainer.IsVisible != isLeft) _leftContainer.IsVisible = isLeft;
        if (_topContainer.IsVisible != isTop) _topContainer.IsVisible = isTop;
        if (_bottomContainer.IsVisible != isBottom) _bottomContainer.IsVisible = isBottom;

    }

    private void MaterializeDataTemplate(ContentPresenter contentView, IDataTemplate? dataTemplate)
    {
        if (dataTemplate is null)
        {
            contentView.ContentTemplate = null;
            contentView.Content = null;
            return;
        }

        if (contentView.ContentTemplate != dataTemplate)
        {
            contentView.ContentTemplate = dataTemplate;
        }

        // Keep content synced so virtualization can rebuild with the current DataContext.
        contentView.Content = DataContext;
    }

    private void PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case PanGestureStatus.Started:
                HandlePanStarted();
                break;

            case PanGestureStatus.Running:
                HandlePanRunning(e);
                break;

            case PanGestureStatus.Completed:
                HandlePanCompleted(e);
                break;
        }
    }

    private void HandlePanStarted()
    {
        _initialX = _currentX;
        _initialY = _currentY;

        _isHorizontalSwipe = false;
        _isVerticalSwipe = false;

        // Materialize all data templates
        MaterializeDataTemplate(_rightContainer, Right);
        MaterializeDataTemplate(_leftContainer, Left);
        MaterializeDataTemplate(_topContainer, Top);
        MaterializeDataTemplate(_bottomContainer, Bottom);

    }

    private void HandlePanRunning(PanUpdatedEventArgs e)
    {
        var absX = Math.Abs(e.TotalX);
        var absY = Math.Abs(e.TotalY);
        var configuredThreshold = Threshold > 0 ? Threshold : DefaultThreshold;
        var isOpenLeft = SwipeState == SwipeState.LeftVisible;
        var isOpenRight = SwipeState == SwipeState.RightVisible;
        var isOpenTop = SwipeState == SwipeState.TopVisible;
        var isOpenBottom = SwipeState == SwipeState.BottomVisible;

        if (!_isHorizontalSwipe && !_isVerticalSwipe)
        {
            if (absX > DirectionLockThreshold || absY > DirectionLockThreshold)
            {
                if (absX > absY)
                {
                    _isHorizontalSwipe = true;
                    _swipeDirection = e.TotalX > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                }
                else
                {
                    _isVerticalSwipe = true;
                    _swipeDirection = e.TotalY > 0 ? SwipeDirection.Down : SwipeDirection.Up;
                }

                RaiseEvent(new SwipeStartedEventArgs(SwipeStartedEvent, _swipeDirection));
            }
        }

        double x = _initialX;
        double y = _initialY;

        if (_isHorizontalSwipe)
        {
            x += e.TotalX;
            
            if (x > 0) // Swiping right (revealing left items)
            {
                if (Left != null && _leftContainer.Bounds.Width > 0)
                {
                    var extent = GetContainerExtent(true, _leftContainer);

                    var maxDistance = extent;
                    x = Math.Min(x, maxDistance);
                }
                else if (Left == null)
                {
                    // No left items, don't allow swiping right
                    x = 0;
                }
            }
            else if (x < 0) // Swiping left (revealing right items)
            {
                if (Right != null && _rightContainer.Bounds.Width > 0)
                {
                    var extent = GetContainerExtent(true, _rightContainer);
                    var maxDistance = extent;
                    x = Math.Max(x, -maxDistance);
                }
                else if (Right == null)
                {
                    // No right items, don't allow swiping left
                    x = 0;
                }
            }
        }
        else if (_isVerticalSwipe)
        {
            y += e.TotalY;
            
            if (y > 0) // Swiping down (revealing top items)
            {
                if (Top != null && _topContainer.Bounds.Height > 0)
                {
                    var extent = GetContainerExtent(false, _topContainer);
                    y = Math.Min(y, extent);
                }
                else if (Top == null)
                {
                    // No top items, don't allow swiping down
                    y = 0;
                }
            }
            else if (y < 0) // Swiping up (revealing bottom items)
            {
                if (Bottom != null && _bottomContainer.Bounds.Height > 0)
                {
                    var extent = GetContainerExtent(false, _bottomContainer);
                    y = Math.Max(y, -extent);
                }
                else if (Bottom == null)
                {
                    // No bottom items, don't allow swiping up
                    y = 0;
                }
            }
        }

        if (_isHorizontalSwipe || _isVerticalSwipe)
        {
            SetTranslate(x, y);

            if (SwipeChanging != null)
            {
                double offset = _isHorizontalSwipe ? x : y;
                _cachedSwipeChangingArgs.SwipeDirection = _swipeDirection;
                _cachedSwipeChangingArgs.Offset = offset;
                SwipeChanging(this, _cachedSwipeChangingArgs);
            }
        }
    }

    private void HandlePanCompleted(PanUpdatedEventArgs e)
    {
        var finalX = _isHorizontalSwipe ? _initialX + e.TotalX : _initialX;
        var finalY = _isVerticalSwipe ? _initialY + e.TotalY : _initialY;
        
        var newState = CalculateState(finalX, finalY);

        if (newState != SwipeState.Hidden)
        {
            SwipeMode activeMode = SwipeMode.Reveal;
            SwipeDirection activeDirection = SwipeDirection.Right;

            switch (newState)
            {
                case SwipeState.LeftVisible:
                    activeMode = LeftMode;
                    activeDirection = SwipeDirection.Right;
                    break;
                case SwipeState.RightVisible:
                    activeMode = RightMode;
                    activeDirection = SwipeDirection.Left;
                    break;
                case SwipeState.TopVisible:
                    activeMode = TopMode;
                    activeDirection = SwipeDirection.Down;
                    break;
                case SwipeState.BottomVisible:
                    activeMode = BottomMode;
                    activeDirection = SwipeDirection.Up;
                    break;
            }

            if (activeMode == SwipeMode.Execute)
            {
                ExecuteRequested?.Invoke(this, activeDirection);
                newState = SwipeState.Hidden;
            }
        }

        if (newState == SwipeState.Hidden && SwipeState == SwipeState.Hidden)
        {
            if (_bodyContainer.Transitions != null && !_bodyContainer.Transitions.Contains(_transition))
                _bodyContainer.Transitions.Add(_transition);

            SetTranslate(0, 0);
        }
        else if (newState == SwipeState)
        {
            // Re-apply the state to snap fully open/closed when the state doesn't change
            ProcessSwipe(newState);
        }
        else
        {
            SetCurrentValue(SwipeStateProperty, newState);
        }

        bool isOpen = newState != SwipeState.Hidden;
        RaiseEvent(new SwipeEndedEventArgs(SwipeEndedEvent, _swipeDirection, isOpen));

        _isHorizontalSwipe = false;
        _isVerticalSwipe = false;
    }

    /// <summary>
    internal void SetSwipeState(SwipeState targetState, bool animated = true)
    {
        var requested = targetState;
        if (requested != SwipeState.Hidden)
        {
            var openItem = requested switch
            {
                SwipeState.LeftVisible => OpenSwipeItem.LeftItems,
                SwipeState.RightVisible => OpenSwipeItem.RightItems,
                SwipeState.TopVisible => OpenSwipeItem.TopItems,
                SwipeState.BottomVisible => OpenSwipeItem.BottomItems,
                _ => OpenSwipeItem.RightItems
            };

            var eventArgs = new OpenRequestedEventArgs(OpenRequestedEvent, openItem);
            RaiseEvent(eventArgs);
            if (eventArgs.Cancel)
                return;
        }
        else
        {
            var eventArgs = new CloseRequestedEventArgs(CloseRequestedEvent);
            RaiseEvent(eventArgs);
            if (eventArgs.Cancel)
                return;
        }

        ApplyStateWithAnimationCheck(animated, () =>
        {
            SetCurrentValue(SwipeStateProperty, requested);
        });
    }

    private void ApplyStateWithAnimationCheck(bool animated, Action action)
    {
        var originalDuration = _transition.Duration;

        _transition.Duration = animated ? AnimationDuration : TimeSpan.Zero;

        action();

        if (!animated)
        {
            Dispatcher.UIThread.Post(() =>
            {
                _transition.Duration = originalDuration;
            }, DispatcherPriority.Render);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_panGestureRecognizer != null)
        {
            _panGestureRecognizer.OnPan -= PanUpdated;
        }
        
        _bodyContainer.RenderTransform = null;
    }
}
