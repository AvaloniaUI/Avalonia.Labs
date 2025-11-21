using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Base.Pan;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media.Transformation;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls;

public class Swipe : Grid
{
    public static readonly StyledProperty<DataTemplate> RightTemplateProperty =
        AvaloniaProperty.Register<Swipe, DataTemplate>(nameof(Right));

    /// <summary>
    /// DataTemplate for the right side
    /// </summary>
    public DataTemplate Right
    {
        get => GetValue(RightTemplateProperty);
        set => SetValue(RightTemplateProperty, value);
    }

    public static readonly StyledProperty<DataTemplate> LeftTemplateProperty =
        AvaloniaProperty.Register<Swipe, DataTemplate>(nameof(Left));

    /// <summary>
    /// DataTemplate for the left side
    /// </summary>
    public DataTemplate Left
    {
        get => GetValue(LeftTemplateProperty);
        set => SetValue(LeftTemplateProperty, value);
    }

    public static readonly StyledProperty<DataTemplate> TopTemplateProperty =
        AvaloniaProperty.Register<Swipe, DataTemplate>(nameof(Top));

    /// <summary>
    /// DataTemplate for the top side
    /// </summary>
    public DataTemplate Top
    {
        get => GetValue(TopTemplateProperty);
        set => SetValue(TopTemplateProperty, value);
    }

    public static readonly StyledProperty<DataTemplate> BottomTemplateProperty =
        AvaloniaProperty.Register<Swipe, DataTemplate>(nameof(Bottom));

    /// <summary>
    /// DataTemplate for the bottom side
    /// </summary>
    public DataTemplate Bottom
    {
        get => GetValue(BottomTemplateProperty);
        set => SetValue(BottomTemplateProperty, value);
    }

    public static readonly StyledProperty<Control> ContentProperty =
        AvaloniaProperty.Register<Swipe, Control>(nameof(Content));

    /// <summary>
    /// The content of the Swipe component
    /// </summary>
    public Control Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<SwipeState> SwipeStateProperty =
        AvaloniaProperty.Register<Swipe, SwipeState>(nameof(SwipeState));

    /// <summary>
    /// The current state of the Swipe component
    /// </summary>
    public SwipeState SwipeState
    {
        get => GetValue(SwipeStateProperty);
        set => SetValue(SwipeStateProperty, value);
    }

    /// <summary>
    /// Routed event for when an open is requested
    /// </summary>
    public static readonly RoutedEvent<OpenRequestedEventArgs> OpenRequestedEvent =
        RoutedEvent.Register<Swipe, OpenRequestedEventArgs>(nameof(OpenRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the swipe is about to open
    /// </summary>
    public event EventHandler<OpenRequestedEventArgs>? OpenRequested
    {
        add => AddHandler(OpenRequestedEvent, value);
        remove => RemoveHandler(OpenRequestedEvent, value);
    }

    /// <summary>
    /// Routed event for when a close is requested
    /// </summary>
    public static readonly RoutedEvent<CloseRequestedEventArgs> CloseRequestedEvent =
        RoutedEvent.Register<Swipe, CloseRequestedEventArgs>(nameof(CloseRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the swipe is about to close
    /// </summary>
    public event EventHandler<CloseRequestedEventArgs>? CloseRequested
    {
        add => AddHandler(CloseRequestedEvent, value);
        remove => RemoveHandler(CloseRequestedEvent, value);
    }

    /// <summary>
    /// Routed event for when a swipe gesture starts
    /// </summary>
    public static readonly RoutedEvent<SwipeStartedEventArgs> SwipeStartedEvent =
        RoutedEvent.Register<Swipe, SwipeStartedEventArgs>(nameof(SwipeStarted), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a swipe gesture begins
    /// </summary>
    public event EventHandler<SwipeStartedEventArgs>? SwipeStarted
    {
        add => AddHandler(SwipeStartedEvent, value);
        remove => RemoveHandler(SwipeStartedEvent, value);
    }

    /// <summary>
    /// Routed event for when a swipe gesture ends
    /// </summary>
    public static readonly RoutedEvent<SwipeEndedEventArgs> SwipeEndedEvent =
        RoutedEvent.Register<Swipe, SwipeEndedEventArgs>(nameof(SwipeEnded), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a swipe gesture completes
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

    private double _initialX;
    private double _currentX;
    private double _initialY;
    private double _currentY;
    private bool _isHorizontalSwipe;
    private bool _isVerticalSwipe;
    private SwipeDirection _swipeDirection;

    public Swipe()
    {
        // Prevent content overflow
        ClipToBounds = true;

        _rightContainer = new ContentPresenter
        {
            IsVisible = false, HorizontalAlignment = HorizontalAlignment.Right
        };

        _leftContainer = new ContentPresenter
        {
            IsVisible = false, HorizontalAlignment = HorizontalAlignment.Left
        };

        _topContainer = new ContentPresenter
        {
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top
        };

        _bottomContainer = new ContentPresenter
        {
            IsVisible = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom
        };

        _bodyContainer = new ContentPresenter
        {
            Transitions = new Transitions()
        };

        _transition = new TransformOperationsTransition
        {
            Property = RenderTransformProperty,
            Duration = TimeSpan.FromMilliseconds(200),
            Easing = new CubicEaseOut()
        };

        _panGestureRecognizer = new PanGestureRecognizer
        {
            Direction = PanDirection.Left | PanDirection.Right | PanDirection.Up | PanDirection.Down,
            Threshold = 10,
        };

        _panGestureRecognizer.OnPan += PanUpdated;

        _bodyContainer.GestureRecognizers.Add(_panGestureRecognizer);

        Children.Add(_rightContainer);
        Children.Add(_leftContainer);
        Children.Add(_topContainer);
        Children.Add(_bottomContainer);
        Children.Add(_bodyContainer);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == ContentProperty)
        {
            _bodyContainer.Content = e.NewValue;
            return;
        }

        if (e.Property == SwipeStateProperty)
        {
            ProcessSwipe(SwipeState);
        }
    }

    private SwipeState CalculateState(double translationX, double translationY)
    {
        // Determine if this is primarily horizontal or vertical swipe
        var absX = Math.Abs(translationX);
        var absY = Math.Abs(translationY);

        if (absX > absY)
        {
            // Horizontal swipe
            var stepSize = translationX < 0
                ? _rightContainer.Bounds.Width
                : _leftContainer.Bounds.Width;

            if (stepSize > absX)
            {
                return SwipeState.Hidden;
            }

            return translationX switch
            {
                < 0 => SwipeState.RightVisible,
                > 0 => SwipeState.LeftVisible,
                _ => SwipeState.Hidden
            };
        }
        else
        {
            // Vertical swipe
            var stepSize = translationY < 0
                ? _bottomContainer.Bounds.Height
                : _topContainer.Bounds.Height;

            if (stepSize > absY)
            {
                return SwipeState.Hidden;
            }

            return translationY switch
            {
                < 0 => SwipeState.BottomVisible,
                > 0 => SwipeState.TopVisible,
                _ => SwipeState.Hidden
            };
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
                break;

            case SwipeState.LeftVisible:
                _leftContainer.IsVisible = true;
                MaterializeDataTemplate(_leftContainer, Left);
                ApplySwipeTransform(() => SetTranslate(_leftContainer.Bounds.Width, 0), _leftContainer);
                break;

            case SwipeState.TopVisible:
                _topContainer.IsVisible = true;
                MaterializeDataTemplate(_topContainer, Top);
                ApplySwipeTransform(() => SetTranslate(0, _topContainer.Bounds.Height), _topContainer);
                break;

            case SwipeState.BottomVisible:
                _bottomContainer.IsVisible = true;
                MaterializeDataTemplate(_bottomContainer, Bottom);
                ApplySwipeTransform(() => SetTranslate(0, -_bottomContainer.Bounds.Height), _bottomContainer);
                break;

            case SwipeState.Hidden:
            default:
                SetTranslate(0, 0);
                break;
        }
    }

    private void ApplySwipeTransform(Action transformAction, ContentPresenter container)
    {
        if (container.Bounds.Width > 0 || container.Bounds.Height > 0)
        {
            // Apply transform immediately
            transformAction();
        }
        else
        {
            // Bounds not available yet, wait for next layout pass
            Dispatcher.UIThread.Post(() =>
            {
                transformAction();
            }, DispatcherPriority.Render);
        }
    }

    private void SetTranslate(double x, double y)
    {
        _currentX = x;
        _currentY = y;

        var transformOperation = TransformOperations.CreateBuilder(1);
        transformOperation.AppendTranslate(x, y);

        _bodyContainer.SetValue(RenderTransformProperty, transformOperation.Build());

        // Update visibility, only show one direction at a time
        var absX = Math.Abs(x);
        var absY = Math.Abs(y);

        if (absX > absY)
        {
            // Horizontal swipe, show left or right only
            _rightContainer.IsVisible = x < 0;
            _leftContainer.IsVisible = x > 0;
            _topContainer.IsVisible = false;
            _bottomContainer.IsVisible = false;
        }
        else if (absY > absX)
        {
            // Vertical swipe, show top or bottom only
            _rightContainer.IsVisible = false;
            _leftContainer.IsVisible = false;
            _topContainer.IsVisible = y > 0;
            _bottomContainer.IsVisible = y < 0;
        }
        else
        {
            // At origin, hide all
            _rightContainer.IsVisible = false;
            _leftContainer.IsVisible = false;
            _topContainer.IsVisible = false;
            _bottomContainer.IsVisible = false;
        }
    }

    private void MaterializeDataTemplate(ContentPresenter contentView, DataTemplate? dataTemplate)
    {
        if (contentView?.Content is not null || dataTemplate is null)
        {
            return;
        }

        try
        {
            var view = dataTemplate.Build(DataContext);
            if (contentView != null)
            {
                contentView.Content = view;
            }
        }
        catch
        {
            // Silently catch template building errors
        }
    }

    private void PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case PanGestureStatus.Started:
                _initialX = _currentX;
                _initialY = _currentY;
                _bodyContainer.Transitions!.Remove(_transition);

                // Reset direction locks
                _isHorizontalSwipe = false;
                _isVerticalSwipe = false;

                // Materialize all templates upfront
                MaterializeDataTemplate(_rightContainer, Right);
                MaterializeDataTemplate(_leftContainer, Left);
                MaterializeDataTemplate(_topContainer, Top);
                MaterializeDataTemplate(_bottomContainer, Bottom);
                break;

            case PanGestureStatus.Running:
                var absX = Math.Abs(e.TotalX);
                var absY = Math.Abs(e.TotalY);

                // Determine direction on first significant movement (threshold of 5 pixels)
                if (!_isHorizontalSwipe && !_isVerticalSwipe && (absX > 5 || absY > 5))
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

                    // Raise SwipeStarted event
                    var swipeStartedArgs = new SwipeStartedEventArgs(SwipeStartedEvent, _swipeDirection);
                    RaiseEvent(swipeStartedArgs);
                }

                // Apply movement only in the locked direction
                var x = _isHorizontalSwipe ? _initialX + e.TotalX : _initialX;
                var y = _isVerticalSwipe ? _initialY + e.TotalY : _initialY;

                SetTranslate(x, y);
                break;

            case PanGestureStatus.Completed:
                _bodyContainer.Transitions!.Add(_transition);

                var finalX = _isHorizontalSwipe ? _initialX + e.TotalX : _initialX;
                var finalY = _isVerticalSwipe ? _initialY + e.TotalY : _initialY;
                var newState = CalculateState(finalX, finalY);

                if (SwipeState == newState)
                {
                    ProcessSwipe(newState);
                }
                else
                {
                    SwipeState = newState;
                }

                // Raise SwipeEnded event
                var isOpen = newState != SwipeState.Hidden;
                var swipeEndedArgs = new SwipeEndedEventArgs(SwipeEndedEvent, _swipeDirection, isOpen);
                RaiseEvent(swipeEndedArgs);

                // Reset direction locks
                _isHorizontalSwipe = false;
                _isVerticalSwipe = false;
                break;
        }
    }

    /// <summary>
    /// Opens the swipe to reveal items in the specified direction
    /// </summary>
    /// <param name="openSwipeItem">The direction to open the swipe</param>
    /// <param name="animated">Whether to animate the opening (default: true)</param>
    public void Open(OpenSwipeItem openSwipeItem, bool animated = true)
    {
        // Raise OpenRequested event
        var eventArgs = new OpenRequestedEventArgs(OpenRequestedEvent, openSwipeItem);
        RaiseEvent(eventArgs);

        // Check if the open was cancelled
        if (eventArgs.Cancel)
        {
            return;
        }

        // Set animation state
        if (animated && !_bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Add(_transition);
        }
        else if (!animated && _bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Remove(_transition);
        }

        // Map OpenSwipeItem to SwipeState
        var newState = openSwipeItem switch
        {
            OpenSwipeItem.LeftItems => SwipeState.LeftVisible,
            OpenSwipeItem.TopItems => SwipeState.TopVisible,
            OpenSwipeItem.RightItems => SwipeState.RightVisible,
            OpenSwipeItem.BottomItems => SwipeState.BottomVisible,
            _ => SwipeState.Hidden
        };

        SwipeState = newState;

        // Restore animation state if it was disabled
        if (!animated && !_bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Add(_transition);
        }
    }

    /// <summary>
    /// Closes the swipe to hide all swipe items
    /// </summary>
    /// <param name="animated">Whether to animate the closing (default: true)</param>
    public void Close(bool animated = true)
    {
        // Raise CloseRequested event
        var eventArgs = new CloseRequestedEventArgs(CloseRequestedEvent);
        RaiseEvent(eventArgs);

        // Check if the close was cancelled
        if (eventArgs.Cancel)
        {
            return;
        }

        // Set animation state
        if (animated && !_bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Add(_transition);
        }
        else if (!animated && _bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Remove(_transition);
        }

        SwipeState = SwipeState.Hidden;

        // Restore animation state if it was disabled
        if (!animated && !_bodyContainer.Transitions!.Contains(_transition))
        {
            _bodyContainer.Transitions!.Add(_transition);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        // Unsubscribe events
        if (_panGestureRecognizer != null)
        {
            _panGestureRecognizer.OnPan -= PanUpdated;
        }

        // Clear transformations
        if (_bodyContainer != null)
        {
            _bodyContainer.RenderTransform = null;
        }
    }
}
