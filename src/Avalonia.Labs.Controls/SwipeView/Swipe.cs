using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Labs.Controls.Base.Pan;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media.Transformation;

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

    private readonly ContentPresenter _rightContainer;
    private readonly ContentPresenter _leftContainer;
    private readonly ContentPresenter _bodyContainer;
    private readonly TransformOperationsTransition _transition;

    private double _initialX;
    private double _currentX;

    public Swipe()
    {
        _rightContainer = new ContentPresenter
        {
            IsVisible = false, HorizontalAlignment = HorizontalAlignment.Right
        };

        _leftContainer = new ContentPresenter
        {
            IsVisible = false, HorizontalAlignment = HorizontalAlignment.Left
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

        var panGestureRecognizer = new PanGestureRecognizer
        {
            Direction = PanDirection.Left | PanDirection.Right, Threshold = 10,
        };

        panGestureRecognizer.OnPan += PanUpdated;

        _bodyContainer.GestureRecognizers.Add(panGestureRecognizer);

        Children.Add(_rightContainer);
        Children.Add(_leftContainer);
        Children.Add(_bodyContainer);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (nameof(Content) == e.Property.Name)
        {
            _bodyContainer.Content = e.NewValue;
            return;
        }

        if (nameof(SwipeState) == e.Property.Name)
        {
            ProcessSwipe(SwipeState);
        }
    }

    private SwipeState CalculateState(double translationX)
    {
        var stepSize = translationX < 0
            ? _rightContainer.Bounds.Width
            : _leftContainer.Bounds.Width;

        if (stepSize > Math.Abs(translationX))
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

    private void ProcessSwipe(SwipeState state)
    {
        switch (state)
        {
            case SwipeState.RightVisible:
                _rightContainer.IsVisible = true;
                MaterializeDataTemplate(_rightContainer, Right);
                SetTranslate(-_rightContainer.Bounds.Width);

                break;
            case SwipeState.LeftVisible:
                _leftContainer.IsVisible = true;
                MaterializeDataTemplate(_leftContainer, Left);
                SetTranslate(_leftContainer.Bounds.Width);

                break;
            case SwipeState.Hidden:
            default:
                SetTranslate(0);
                break;
        }
    }

    private void SetTranslate(double x)
    {
        _currentX = x;
        var transformOperation = TransformOperations.CreateBuilder(1);
        transformOperation.AppendTranslate(x, 0);

        _bodyContainer.SetValue(RenderTransformProperty, transformOperation.Build());
    }

    private void MaterializeDataTemplate(ContentPresenter contentView, DataTemplate? dataTemplate)
    {
        if (contentView.Content is not null || dataTemplate is null)
        {
            return;
        }

        var view = dataTemplate.Build(DataContext);
        contentView.Content = view;
    }

    private void PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case PanGestureStatus.Started:
                _initialX = _currentX;
                _bodyContainer.Transitions!.Remove(_transition);
                MaterializeDataTemplate(_rightContainer, Right);
                MaterializeDataTemplate(_leftContainer, Left);

                break;
            case PanGestureStatus.Running:
                var x = _initialX + e.TotalX;

                SetTranslate(x);

                _rightContainer.IsVisible = x < 0;
                _leftContainer.IsVisible = x > 0;
                break;
            case PanGestureStatus.Completed:
                _bodyContainer.Transitions!.Add(_transition);
                var newState = CalculateState(_initialX + e.TotalX);
                if (SwipeState == newState)
                {
                    ProcessSwipe(newState);
                    return;
                }

                SwipeState = newState;
                break;
        }
    }
}
