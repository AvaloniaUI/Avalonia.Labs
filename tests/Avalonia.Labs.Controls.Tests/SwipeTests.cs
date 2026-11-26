using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Automation.Peers;
using Avalonia.Labs.Controls.Base.Pan;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Labs.Controls.Tests;

public class SwipeTests
{
    [AvaloniaFact]
    public void Swipe_Initializes_With_Correct_Defaults()
    {
        var swipe = new Swipe();

        Assert.NotNull(swipe);
        Assert.Null(swipe.Left);
        Assert.Null(swipe.Right);
        Assert.Null(swipe.Top);
        Assert.Null(swipe.Bottom);
        Assert.Null(swipe.Content);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Swipe_Has_Default_Threshold_Of_100()
    {
        var swipe = new Swipe();

        Assert.Equal(100.0, swipe.Threshold);
    }

    [AvaloniaFact]
    public void Swipe_Has_Default_AnimationDuration_Of_200ms()
    {
        var swipe = new Swipe();

        Assert.Equal(TimeSpan.FromMilliseconds(200), swipe.AnimationDuration);
    }

    [AvaloniaFact]
    public void Swipe_Has_Default_SwipeModes_As_Reveal()
    {
        var swipe = new Swipe();

        Assert.Equal(SwipeMode.Reveal, swipe.LeftMode);
        Assert.Equal(SwipeMode.Reveal, swipe.RightMode);
        Assert.Equal(SwipeMode.Reveal, swipe.TopMode);
        Assert.Equal(SwipeMode.Reveal, swipe.BottomMode);
    }

    [AvaloniaFact]
    public void Swipe_ClipToBounds_Is_True()
    {
        var swipe = new Swipe();

        Assert.True(swipe.ClipToBounds);
    }

    [AvaloniaFact]
    public void Swipe_Is_Focusable()
    {
        var swipe = new Swipe();

        Assert.True(swipe.Focusable);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Left_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Left = template;

        Assert.Equal(template, swipe.Left);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Right_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Right = template;

        Assert.Equal(template, swipe.Right);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Top_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Top = template;

        Assert.Equal(template, swipe.Top);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Bottom_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Bottom = template;

        Assert.Equal(template, swipe.Bottom);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Content()
    {
        var swipe = new Swipe();
        var content = new TextBlock { Text = "Test" };

        swipe.Content = content;

        Assert.Equal(content, swipe.Content);
    }

    [AvaloniaFact]
    public void Swipe_Can_Clear_Templates()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Left = template;
        swipe.Left = null;

        Assert.Null(swipe.Left);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_Threshold()
    {
        var swipe = new Swipe();

        swipe.Threshold = 150.0;

        Assert.Equal(150.0, swipe.Threshold);
    }

    [AvaloniaFact]
    public void Swipe_Threshold_Can_Be_Zero()
    {
        var swipe = new Swipe();

        swipe.Threshold = 0;

        Assert.Equal(0, swipe.Threshold);
    }

    [AvaloniaFact]
    public void Swipe_Threshold_Can_Be_Negative()
    {
        var swipe = new Swipe();

        swipe.Threshold = -50.0;

        Assert.Equal(-50.0, swipe.Threshold);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_AnimationDuration()
    {
        var swipe = new Swipe();

        swipe.AnimationDuration = TimeSpan.FromMilliseconds(500);

        Assert.Equal(TimeSpan.FromMilliseconds(500), swipe.AnimationDuration);
    }

    [AvaloniaFact]
    public void Swipe_AnimationDuration_Can_Be_Zero()
    {
        var swipe = new Swipe();

        swipe.AnimationDuration = TimeSpan.Zero;

        Assert.Equal(TimeSpan.Zero, swipe.AnimationDuration);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_LeftMode()
    {
        var swipe = new Swipe();

        swipe.LeftMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.LeftMode);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_RightMode()
    {
        var swipe = new Swipe();

        swipe.RightMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.RightMode);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_TopMode()
    {
        var swipe = new Swipe();

        swipe.TopMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.TopMode);
    }

    [AvaloniaFact]
    public void Swipe_Can_Set_BottomMode()
    {
        var swipe = new Swipe();

        swipe.BottomMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.BottomMode);
    }

    [AvaloniaFact]
    public void SwipeMode_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<SwipeMode>();

        Assert.Contains(SwipeMode.Reveal, values);
        Assert.Contains(SwipeMode.Execute, values);
        Assert.Equal(2, values.Length);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Be_Set_To_LeftVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;

        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Be_Set_To_RightVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.RightVisible;

        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Be_Set_To_TopVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.TopVisible;

        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Be_Set_To_BottomVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.BottomVisible;

        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Be_Set_To_Hidden()
    {
        var swipe = new Swipe();
        swipe.SwipeState = SwipeState.LeftVisible;

        swipe.SwipeState = SwipeState.Hidden;

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Enum_Has_All_Expected_Values()
    {
        var values = Enum.GetValues<SwipeState>();

        Assert.Contains(SwipeState.Hidden, values);
        Assert.Contains(SwipeState.LeftVisible, values);
        Assert.Contains(SwipeState.RightVisible, values);
        Assert.Contains(SwipeState.TopVisible, values);
        Assert.Contains(SwipeState.BottomVisible, values);
        Assert.Equal(5, values.Length);
    }

    [AvaloniaFact]
    public void SwipeState_Hidden_Is_Default_Value()
    {
        Assert.Equal(0, (int)SwipeState.Hidden);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Transition_Through_All_States()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.TopVisible;
        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.BottomVisible;
        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void SwipeState_Can_Switch_Directly_Between_Visible_States()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        // Switch directly without going through Hidden
        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.TopVisible;
        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.BottomVisible;
        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Multiple_SwipeState_Cycles_Work_Correctly()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);

        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);

        swipe.SwipeState = SwipeState.TopVisible;
        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);

        swipe.SwipeState = SwipeState.BottomVisible;
        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Setting_SwipeState_To_Current_Value_Does_Not_Throw()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.Hidden;
        swipe.SwipeState = SwipeState.Hidden;

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void LeftTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.LeftTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Left", property.Name);
    }

    [AvaloniaFact]
    public void RightTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.RightTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Right", property.Name);
    }

    [AvaloniaFact]
    public void TopTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.TopTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Top", property.Name);
    }

    [AvaloniaFact]
    public void BottomTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.BottomTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Bottom", property.Name);
    }

    [AvaloniaFact]
    public void ContentProperty_Is_StyledProperty()
    {
        var property = Swipe.ContentProperty;

        Assert.NotNull(property);
        Assert.Equal("Content", property.Name);
    }

    [AvaloniaFact]
    public void SwipeStateProperty_Is_StyledProperty()
    {
        var property = Swipe.SwipeStateProperty;

        Assert.NotNull(property);
        Assert.Equal("SwipeState", property.Name);
    }

    [AvaloniaFact]
    public void ThresholdProperty_Is_StyledProperty()
    {
        var property = Swipe.ThresholdProperty;

        Assert.NotNull(property);
        Assert.Equal("Threshold", property.Name);
    }

    [AvaloniaFact]
    public void AnimationDurationProperty_Is_StyledProperty()
    {
        var property = Swipe.AnimationDurationProperty;

        Assert.NotNull(property);
        Assert.Equal("AnimationDuration", property.Name);
    }

    [AvaloniaFact]
    public void LeftModeProperty_Is_StyledProperty()
    {
        var property = Swipe.LeftModeProperty;

        Assert.NotNull(property);
        Assert.Equal("LeftMode", property.Name);
    }

    [AvaloniaFact]
    public void RightModeProperty_Is_StyledProperty()
    {
        var property = Swipe.RightModeProperty;

        Assert.NotNull(property);
        Assert.Equal("RightMode", property.Name);
    }

    [AvaloniaFact]
    public void TopModeProperty_Is_StyledProperty()
    {
        var property = Swipe.TopModeProperty;

        Assert.NotNull(property);
        Assert.Equal("TopMode", property.Name);
    }

    [AvaloniaFact]
    public void BottomModeProperty_Is_StyledProperty()
    {
        var property = Swipe.BottomModeProperty;

        Assert.NotNull(property);
        Assert.Equal("BottomMode", property.Name);
    }

    [AvaloniaFact]
    public void OpenSwipeItem_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<OpenSwipeItem>();

        Assert.Contains(OpenSwipeItem.LeftItems, values);
        Assert.Contains(OpenSwipeItem.RightItems, values);
        Assert.Contains(OpenSwipeItem.TopItems, values);
        Assert.Contains(OpenSwipeItem.BottomItems, values);
        Assert.Equal(4, values.Length);
    }

    [AvaloniaFact]
    public void SwipeDirection_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<SwipeDirection>();

        Assert.Contains(SwipeDirection.Left, values);
        Assert.Contains(SwipeDirection.Right, values);
        Assert.Contains(SwipeDirection.Up, values);
        Assert.Contains(SwipeDirection.Down, values);
        Assert.Equal(4, values.Length);
    }

    [AvaloniaFact]
    public void OpenRequestedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.OpenRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("OpenRequested", eventInfo.Name);
    }

    [AvaloniaFact]
    public void CloseRequestedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.CloseRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("CloseRequested", eventInfo.Name);
    }

    [AvaloniaFact]
    public void SwipeStartedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.SwipeStartedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("SwipeStarted", eventInfo.Name);
    }

    [AvaloniaFact]
    public void SwipeEndedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.SwipeEndedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("SwipeEnded", eventInfo.Name);
    }

    [AvaloniaFact]
    public void OpenRequestedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.OpenRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [AvaloniaFact]
    public void CloseRequestedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.CloseRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [AvaloniaFact]
    public void SwipeStartedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.SwipeStartedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [AvaloniaFact]
    public void SwipeEndedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.SwipeEndedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [AvaloniaFact]
    public void OpenRequestedEventArgs_Has_Cancel_Property()
    {
        var args = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.LeftItems);

        Assert.False(args.Cancel);
        args.Cancel = true;
        Assert.True(args.Cancel);
    }

    [AvaloniaFact]
    public void OpenRequestedEventArgs_Has_OpenSwipeItem_Property()
    {
        var args = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.RightItems);

        Assert.Equal(OpenSwipeItem.RightItems, args.OpenSwipeItem);
    }

    [AvaloniaFact]
    public void OpenRequestedEventArgs_Preserves_All_OpenSwipeItem_Values()
    {
        var leftArgs = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.LeftItems);
        var rightArgs = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.RightItems);
        var topArgs = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.TopItems);
        var bottomArgs = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.BottomItems);

        Assert.Equal(OpenSwipeItem.LeftItems, leftArgs.OpenSwipeItem);
        Assert.Equal(OpenSwipeItem.RightItems, rightArgs.OpenSwipeItem);
        Assert.Equal(OpenSwipeItem.TopItems, topArgs.OpenSwipeItem);
        Assert.Equal(OpenSwipeItem.BottomItems, bottomArgs.OpenSwipeItem);
    }

    [AvaloniaFact]
    public void CloseRequestedEventArgs_Has_Cancel_Property()
    {
        var args = new CloseRequestedEventArgs(Swipe.CloseRequestedEvent);

        Assert.False(args.Cancel);
        args.Cancel = true;
        Assert.True(args.Cancel);
    }

    [AvaloniaFact]
    public void SwipeStartedEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Left);

        Assert.Equal(SwipeDirection.Left, args.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeStartedEventArgs_Preserves_All_SwipeDirection_Values()
    {
        var leftArgs = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Left);
        var rightArgs = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Right);
        var upArgs = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Up);
        var downArgs = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Down);

        Assert.Equal(SwipeDirection.Left, leftArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Right, rightArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Up, upArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Down, downArgs.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeEndedEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Right, true);

        Assert.Equal(SwipeDirection.Right, args.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeEndedEventArgs_Has_IsOpen_Property()
    {
        var argsOpen = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Left, true);
        var argsClosed = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Left, false);

        Assert.True(argsOpen.IsOpen);
        Assert.False(argsClosed.IsOpen);
    }

    [AvaloniaFact]
    public void SwipeEndedEventArgs_Preserves_All_SwipeDirection_Values()
    {
        var leftArgs = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Left, true);
        var rightArgs = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Right, false);
        var upArgs = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Up, true);
        var downArgs = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Down, false);

        Assert.Equal(SwipeDirection.Left, leftArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Right, rightArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Up, upArgs.SwipeDirection);
        Assert.Equal(SwipeDirection.Down, downArgs.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Up, 50.0);

        Assert.Equal(SwipeDirection.Up, args.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_Has_Offset_Property()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Down, 75.5);

        Assert.Equal(75.5, args.Offset);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_SwipeDirection_Is_Mutable()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, 0);

        args.SwipeDirection = SwipeDirection.Right;

        Assert.Equal(SwipeDirection.Right, args.SwipeDirection);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_Offset_Is_Mutable()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, 50.0);

        args.Offset = 100.0;

        Assert.Equal(100.0, args.Offset);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_Offset_Can_Be_Negative()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, -50.0);

        Assert.Equal(-50.0, args.Offset);
    }

    [AvaloniaFact]
    public void SwipeChangingEventArgs_Offset_Can_Be_Zero()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Right, 0);

        Assert.Equal(0, args.Offset);
    }

    [AvaloniaFact]
    public void Swipe_Inherits_DataContext()
    {
        var swipe = new Swipe();
        var dataContext = new { Name = "Test" };

        swipe.DataContext = dataContext;

        Assert.Same(dataContext, swipe.DataContext);
    }

    [AvaloniaFact]
    public void Swipe_DataContext_Can_Be_Null()
    {
        var swipe = new Swipe();
        swipe.DataContext = new { Name = "Test" };

        swipe.DataContext = null;

        Assert.Null(swipe.DataContext);
    }

    [AvaloniaFact]
    public void Swipe_Has_Five_Children_After_Construction()
    {
        var swipe = new Swipe();

        // Should have 4 side containers + 1 body container
        Assert.Equal(5, swipe.Children.Count);
    }

    [AvaloniaFact]
    public void Swipe_Inherits_From_Grid()
    {
        var swipe = new Swipe();

        Assert.IsAssignableFrom<Grid>(swipe);
    }

    [AvaloniaFact]
    public void Content_Can_Be_Changed_Multiple_Times()
    {
        var swipe = new Swipe();
        var content1 = new TextBlock { Text = "First" };
        var content2 = new Button { Content = "Second" };

        swipe.Content = content1;
        Assert.Equal(content1, swipe.Content);

        swipe.Content = content2;
        Assert.Equal(content2, swipe.Content);

        swipe.Content = null;
        Assert.Null(swipe.Content);
    }

    [AvaloniaFact]
    public void ThresholdProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.ThresholdProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(100.0, defaultValue);
    }

    [AvaloniaFact]
    public void AnimationDurationProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.AnimationDurationProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(TimeSpan.FromMilliseconds(200), defaultValue);
    }

    [AvaloniaFact]
    public void LeftModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.LeftModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [AvaloniaFact]
    public void RightModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.RightModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [AvaloniaFact]
    public void TopModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.TopModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [AvaloniaFact]
    public void BottomModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.BottomModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }


    [AvaloniaFact]
    public void Direct_child_element_syntax_in_XAML()
    {
        // If Dispatcher has a thread, invoke on it
        var xaml = $"""
            <labs:Swipe xmlns="https://github.com/avaloniaui"
                xmlns:labs="using:Avalonia.Labs.Controls">
                <TextBlock Text="Direct Child Content" />
            </labs:Swipe>
            """;

        Swipe? swipe = null;
        if (Dispatcher.UIThread.CheckAccess())
        {
            swipe = AvaloniaRuntimeXamlLoader.Load(xaml) as Swipe;
        }
        else
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                swipe = AvaloniaRuntimeXamlLoader.Load(xaml) as Swipe;
            });
        }

        Assert.NotNull(swipe);
        var tb = Assert.IsType<TextBlock>(swipe.Content);
        Assert.Equal("Direct Child Content", tb.Text);
    }


    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_False_Programmatic_Swiping_Continues_To_Work()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = false
        };

        // Setting SwipeState directly should work even when IsSwipeEnabled is false
        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.TopVisible;
        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.BottomVisible;
        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_False_Keyboard_Gestures_Are_Disabled()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = false
        };

        var openRequestedCount = 0;
        swipe.OpenRequested += (_, _) => openRequestedCount++;

        // Try to open with Ctrl+Left
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Left,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(0, openRequestedCount);

        // Try to open with Ctrl+Right
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Right,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(0, openRequestedCount);

        // Try to open with Ctrl+Up
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Up,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(0, openRequestedCount);

        // Try to open with Ctrl+Down
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Down,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(0, openRequestedCount);
    }

    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_False_Escape_Key_Is_Disabled()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = false,
            SwipeState = SwipeState.LeftVisible
        };

        var closeRequestedCount = 0;
        swipe.CloseRequested += (_, _) => closeRequestedCount++;

        // Press Escape - should not close when IsSwipeEnabled is false
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Escape
        });

        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
        Assert.Equal(0, closeRequestedCount);
    }

    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_True_Keyboard_Gestures_Are_Enabled()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = true
        };

        var openedItems = new List<OpenSwipeItem>();
        swipe.OpenRequested += (_, e) => openedItems.Add(e.OpenSwipeItem);

        // Ctrl+Left -> LeftVisible
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Left,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
        Assert.Contains(OpenSwipeItem.LeftItems, openedItems);

        // Ctrl+Right -> RightVisible
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Right,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
        Assert.Contains(OpenSwipeItem.RightItems, openedItems);

        // Ctrl+Up -> TopVisible
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Up,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);
        Assert.Contains(OpenSwipeItem.TopItems, openedItems);

        // Ctrl+Down -> BottomVisible
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Down,
            KeyModifiers = KeyModifiers.Control
        });
        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);
        Assert.Contains(OpenSwipeItem.BottomItems, openedItems);

        // Escape -> Hidden
        var closeRequestedCount = 0;
        swipe.CloseRequested += (_, _) => closeRequestedCount++;
        swipe.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Escape
        });
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(1, closeRequestedCount);
    }

    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_False_SetSwipeState_Does_Nothing()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = false
        };

        var openRequestedCount = 0;
        swipe.OpenRequested += (_, _) => openRequestedCount++;

        var setSwipeStateMethod = typeof(Swipe).GetMethod("SetSwipeState", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(setSwipeStateMethod);

        setSwipeStateMethod.Invoke(swipe, new object[] { SwipeState.LeftVisible, true });

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Equal(0, openRequestedCount);
    }


    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_False_Pan_Gestures_Are_Disabled()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = false
        };

        var swipeStarted = false;
        var swipeChanging = false;
        var swipeEnded = false;

        swipe.SwipeStarted += (_, _) => swipeStarted = true;
        swipe.SwipeChanging += (_, _) => swipeChanging = true;
        swipe.SwipeEnded += (_, _) => swipeEnded = true;

        var panUpdatedMethod = typeof(Swipe).GetMethod("PanUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(panUpdatedMethod);

        // Simulate Started
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0)]);
        // Simulate Running
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Running, 50, 0)]);
        // Simulate Completed
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Completed, 150, 0)]);

        Assert.False(swipeStarted);
        Assert.False(swipeChanging);
        Assert.False(swipeEnded);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void When_IsSwipeEnabled_Is_True_Pan_Gestures_Are_Processed()
    {
        var swipe = new Swipe
        {
            IsSwipeEnabled = true
        };

        var swipeStarted = false;
        var swipeChanging = false;
        var swipeEnded = false;

        swipe.SwipeStarted += (_, _) => swipeStarted = true;
        swipe.SwipeChanging += (_, _) => swipeChanging = true;
        swipe.SwipeEnded += (_, _) => swipeEnded = true;

        var panUpdatedMethod = typeof(Swipe).GetMethod("PanUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(panUpdatedMethod);

        // Simulate Started
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0)]);
        // Simulate Running with offset beyond DirectionLockThreshold (5)
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Running, 50, 0)]);
        // Simulate Completed
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Completed, 150, 0)]);

        Assert.True(swipeStarted);
        Assert.True(swipeChanging);
        Assert.True(swipeEnded);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureData))]
    public void Command_Is_Invoked_On_Swipe_Gesture_When_Mode_Is_Execute_And_Template_Is_Set(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY,
        SwipeDirection expectedDirection,
        object expectedParam)
    {
        object? receivedParam = null;
        var executed = false;
        var command = new TestCommand(param =>
        {
            executed = true;
            receivedParam = param;
        });

        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        configure(swipe, template, SwipeMode.Execute, command, expectedParam);

        var swipeEndedCalled = false;
        SwipeEndedEventArgs? endedArgs = null;
        swipe.SwipeEnded += (_, e) =>
        {
            swipeEndedCalled = true;
            endedArgs = e;
        };

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.True(executed);
        Assert.Equal(expectedParam, receivedParam);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.True(swipeEndedCalled);
        Assert.NotNull(endedArgs);
        Assert.False(endedArgs.IsOpen);
        Assert.Equal(expectedDirection, endedArgs.SwipeDirection);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureSimpleData))]
    public void Command_Is_Not_Invoked_When_CanExecute_Returns_False(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY)
    {
        var executed = false;
        var command = new TestCommand(
            execute: _ => executed = true,
            canExecute: _ => false);

        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        configure(swipe, template, SwipeMode.Execute, command, "arg");

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.False(executed);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureSimpleData))]
    public void Command_Is_Not_Invoked_When_Mode_Is_Reveal(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY)
    {
        var executed = false;
        var command = new TestCommand(execute: _ => executed = true);

        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        configure(swipe, template, SwipeMode.Reveal, command, null);

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.False(executed);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureSimpleData))]
    public void Command_Is_Not_Invoked_When_Template_Is_Not_Set(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY)
    {
        var executed = false;
        var command = new TestCommand(execute: _ => executed = true);

        var swipe = new Swipe();
        configure(swipe, null, SwipeMode.Execute, command, null);

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.False(executed);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureSimpleData))]
    public void When_Command_Is_Null_And_Mode_Is_Execute_SwipeState_Returns_To_Hidden(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY)
    {
        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        configure(swipe, template, SwipeMode.Execute, null, null);

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaTheory]
    [MemberData(nameof(DirectionGestureSimpleData))]
    public void Commands_Are_Not_Invoked_When_IsSwipeEnabled_Is_False(
        Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?> configure,
        double deltaX,
        double deltaY)
    {
        var executed = false;
        var swipe = new Swipe
        {
            IsSwipeEnabled = false
        };
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        configure(swipe, template, SwipeMode.Execute, new TestCommand(_ => executed = true), "test_arg");

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.False(executed);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    public static IEnumerable<object[]> DirectionGestureData()
    {
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Left = t;
                s.LeftMode = m;
                s.LeftCommand = c;
                s.LeftCommandParameter = p;
            }),
            150.0, 0.0,
            SwipeDirection.Right,
            "custom_left_arg"
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Right = t;
                s.RightMode = m;
                s.RightCommand = c;
                s.RightCommandParameter = p;
            }),
            -150.0, 0.0,
            SwipeDirection.Left,
            42
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Top = t;
                s.TopMode = m;
                s.TopCommand = c;
                s.TopCommandParameter = p;
            }),
            0.0, 150.0,
            SwipeDirection.Down,
            "top_value"
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Bottom = t;
                s.BottomMode = m;
                s.BottomCommand = c;
                s.BottomCommandParameter = p;
            }),
            0.0, -150.0,
            SwipeDirection.Up,
            "bottom_value"
        };
    }

    public static IEnumerable<object[]> DirectionGestureSimpleData()
    {
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Left = t;
                s.LeftMode = m;
                s.LeftCommand = c;
                s.LeftCommandParameter = p;
            }),
            150.0, 0.0
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Right = t;
                s.RightMode = m;
                s.RightCommand = c;
                s.RightCommandParameter = p;
            }),
            -150.0, 0.0
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Top = t;
                s.TopMode = m;
                s.TopCommand = c;
                s.TopCommandParameter = p;
            }),
            0.0, 150.0
        };
        yield return new object[]
        {
            new Action<Swipe, IDataTemplate?, SwipeMode, ICommand?, object?>((s, t, m, c, p) =>
            {
                s.Bottom = t;
                s.BottomMode = m;
                s.BottomCommand = c;
                s.BottomCommandParameter = p;
            }),
            0.0, -150.0
        };
    }

    private static void SimulatePanGesture(Swipe swipe, double deltaX, double deltaY)
    {
        swipe.DataContext ??= new object();

        var panUpdatedMethod = typeof(Swipe).GetMethod("PanUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(panUpdatedMethod);

        // 1. Started
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Started, 0, 0)]);

        // 2. Running (pass initial lock threshold to make active side container visible)
        var runningX = deltaX != 0 ? (deltaX > 0 ? 50 : -50) : 0;
        var runningY = deltaY != 0 ? (deltaY > 0 ? 50 : -50) : 0;
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Running, runningX, runningY)]);

        // Set dimensions on visible side containers for unit testing
        foreach (var child in swipe.Children)
        {
            if (child is ContentPresenter cp && cp.IsVisible)
            {
                cp.Width = 100;
                cp.Height = 100;
                cp.Measure(new Size(500, 500));
            }
        }

        // 3. Completed
        panUpdatedMethod.Invoke(swipe, [null, new PanUpdatedEventArgs(PanGestureStatus.Completed, deltaX, deltaY)]);
    }

    [AvaloniaTheory]
    [InlineData(150.0, 0.0, OpenSwipeItem.LeftItems, SwipeState.LeftVisible)]
    [InlineData(-150.0, 0.0, OpenSwipeItem.RightItems, SwipeState.RightVisible)]
    [InlineData(0.0, 150.0, OpenSwipeItem.TopItems, SwipeState.TopVisible)]
    [InlineData(0.0, -150.0, OpenSwipeItem.BottomItems, SwipeState.BottomVisible)]
    public void PanGesture_Raises_OpenRequested_Event(double deltaX, double deltaY, OpenSwipeItem expectedItem, SwipeState expectedState)
    {
        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        swipe.Left = template;
        swipe.Right = template;
        swipe.Top = template;
        swipe.Bottom = template;

        OpenRequestedEventArgs? openArgs = null;
        swipe.OpenRequested += (_, e) => openArgs = e;

        SimulatePanGesture(swipe, deltaX, deltaY);

        Assert.NotNull(openArgs);
        Assert.Equal(expectedItem, openArgs.OpenSwipeItem);
        Assert.Equal(expectedState, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void PanGesture_OpenRequested_Cancel_Prevents_Opening()
    {
        var swipe = new Swipe();
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        swipe.Left = template;

        var openRequestedFired = false;
        swipe.OpenRequested += (_, e) =>
        {
            openRequestedFired = true;
            e.Cancel = true;
        };

        SimulatePanGesture(swipe, 150.0, 0.0);

        Assert.True(openRequestedFired);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void PanGesture_Raises_CloseRequested_Event()
    {
        var swipe = new Swipe
        {
            SwipeState = SwipeState.LeftVisible
        };
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        swipe.Left = template;

        var closeRequestedFired = false;
        swipe.CloseRequested += (_, _) => closeRequestedFired = true;

        // Swiping left to close
        SimulatePanGesture(swipe, -150.0, 0.0);

        Assert.True(closeRequestedFired);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void PanGesture_CloseRequested_Cancel_Prevents_Closing()
    {
        var swipe = new Swipe
        {
            SwipeState = SwipeState.LeftVisible
        };
        var template = new FuncDataTemplate<object?>((_, _) => new Border { Width = 100, Height = 100 });
        swipe.Left = template;

        var closeRequestedFired = false;
        swipe.CloseRequested += (_, e) =>
        {
            closeRequestedFired = true;
            e.Cancel = true;
        };

        // Swiping left to close
        SimulatePanGesture(swipe, -150.0, 0.0);

        Assert.True(closeRequestedFired);
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
    }

    [AvaloniaTheory]
    [InlineData(OpenSwipeItem.LeftItems, SwipeState.LeftVisible)]
    [InlineData(OpenSwipeItem.RightItems, SwipeState.RightVisible)]
    [InlineData(OpenSwipeItem.TopItems, SwipeState.TopVisible)]
    [InlineData(OpenSwipeItem.BottomItems, SwipeState.BottomVisible)]
    public void Programmatic_Open_Method_Raises_OpenRequested(OpenSwipeItem item, SwipeState expectedState)
    {
        var swipe = new Swipe();
        OpenRequestedEventArgs? openArgs = null;
        swipe.OpenRequested += (_, e) => openArgs = e;

        swipe.Open(item, false);

        Assert.NotNull(openArgs);
        Assert.Equal(item, openArgs.OpenSwipeItem);
        Assert.Equal(expectedState, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Programmatic_Open_Method_Can_Be_Cancelled()
    {
        var swipe = new Swipe();
        swipe.OpenRequested += (_, e) => e.Cancel = true;

        swipe.Open(OpenSwipeItem.LeftItems, false);

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Programmatic_Close_Method_Raises_CloseRequested()
    {
        var swipe = new Swipe
        {
            SwipeState = SwipeState.LeftVisible
        };

        var closeRequestedFired = false;
        swipe.CloseRequested += (_, _) => closeRequestedFired = true;

        swipe.Close(false);

        Assert.True(closeRequestedFired);
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Programmatic_Close_Method_Can_Be_Cancelled()
    {
        var swipe = new Swipe
        {
            SwipeState = SwipeState.LeftVisible
        };

        swipe.CloseRequested += (_, e) => e.Cancel = true;

        swipe.Close(false);

        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
    }

    [AvaloniaFact]
    public void Programmatic_SwipeState_Direct_Assignment_Raises_OpenRequested_And_CloseRequested()
    {
        var swipe = new Swipe();
        var openedItems = new List<OpenSwipeItem>();
        var closeCount = 0;

        swipe.OpenRequested += (_, e) => openedItems.Add(e.OpenSwipeItem);
        swipe.CloseRequested += (_, _) => closeCount++;

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(new[] { OpenSwipeItem.LeftItems }, openedItems);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(1, closeCount);
    }

    [AvaloniaFact]
    public void Programmatic_SwipeState_Direct_Assignment_Cancel_Reverts_State()
    {
        var swipe = new Swipe();
        swipe.OpenRequested += (_, e) => e.Cancel = true;

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    private class TestCommand : ICommand
    {
        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;

        public TestCommand(Action<object?>? execute = null, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute?.Invoke(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    [AvaloniaFact]
    public void Swipe_Creates_SwipeAutomationPeer()
    {
        var swipe = new Swipe();
        var peer = ControlAutomationPeer.CreatePeerForElement(swipe);

        Assert.NotNull(peer);
        Assert.IsType<SwipeAutomationPeer>(peer);
    }

    [AvaloniaFact]
    public void SwipeAutomationPeer_Properties_And_Provider_Work_Correctly()
    {
        var swipe = new Swipe();
        var peer = new SwipeAutomationPeer(swipe);

        Assert.Equal(swipe, peer.Owner);
        Assert.Equal(AutomationControlType.Pane, peer.GetAutomationControlType());
        Assert.Equal("Swipe", peer.GetClassName());
        Assert.False(peer.CanSelectMultiple);
        Assert.False(peer.IsSelectionRequired);
        Assert.Empty(peer.GetSelection());
    }

    [AvaloniaFact]
    public void SwipeAutomationPeer_GetProvider_Returns_SelectionProvider()
    {
        var swipe = new Swipe();
        var peer = new SwipeAutomationPeer(swipe);

        var provider = peer.GetProvider<ISelectionProvider>();
        Assert.NotNull(provider);
        Assert.Same(peer, provider);
    }

    [AvaloniaFact]
    public void SwipeAutomationPeer_Selection_Works_Correctly()
    {
        var swipe = new Swipe
        {
            Left = new DataTemplate(),
            Right = new DataTemplate()
        };
        var peer = new SwipeAutomationPeer(swipe);

        Assert.False(peer.CanSelectMultiple);
        Assert.False(peer.IsSelectionRequired);
        Assert.Empty(peer.GetSelection());

        // Select Left
        swipe.Open(OpenSwipeItem.LeftItems);
        var selection = peer.GetSelection();
        Assert.Single(selection);
        var itemPeer = Assert.IsType<SwipeItemAutomationPeer>(selection[0]);
        Assert.Equal(OpenSwipeItem.LeftItems, itemPeer.SwipeItem);
        Assert.True(itemPeer.IsSelected);
        Assert.Same(peer, itemPeer.SelectionContainer);

        // Deselect via RemoveFromSelection
        itemPeer.RemoveFromSelection();
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        Assert.Empty(peer.GetSelection());

        // Select via ItemPeer.Select()
        var rightItemPeer = peer.GetOrCreateItemPeer(OpenSwipeItem.RightItems);
        rightItemPeer.Select();
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
        Assert.True(rightItemPeer.IsSelected);
        Assert.Single(peer.GetSelection());
    }

    [AvaloniaFact]
    public void SwipeAutomationPeer_GetChildren_Includes_Configured_Items()
    {
        var swipe = new Swipe
        {
            Left = new DataTemplate(),
            Top = new DataTemplate(),
            Right = new DataTemplate(),
            Bottom = new DataTemplate()
        };
        var peer = new SwipeAutomationPeer(swipe);

        var children = peer.GetChildren();
        Assert.True(children.Count >= 4);
        Assert.Contains(children, c => c is SwipeItemAutomationPeer { SwipeItem: OpenSwipeItem.LeftItems });
        Assert.Contains(children, c => c is SwipeItemAutomationPeer { SwipeItem: OpenSwipeItem.TopItems });
        Assert.Contains(children, c => c is SwipeItemAutomationPeer { SwipeItem: OpenSwipeItem.RightItems });
        Assert.Contains(children, c => c is SwipeItemAutomationPeer { SwipeItem: OpenSwipeItem.BottomItems });
    }

    private static PanDirection GetPanDirection(Swipe swipe)
    {
        var field = typeof(Swipe).GetField("_panGestureRecognizer", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        var recognizer = (PanGestureRecognizer)field.GetValue(swipe)!;
        Assert.NotNull(recognizer);
        return recognizer.Direction;
    }

    [AvaloniaFact]
    public void PanGestureRecognizer_Defaults_To_All_Directions()
    {
        var recognizer = new PanGestureRecognizer();

        Assert.Equal(
            PanDirection.Left | PanDirection.Right | PanDirection.Up | PanDirection.Down,
            recognizer.Direction);
        Assert.Equal(5, recognizer.Threshold);
    }

    [AvaloniaFact]
    public void Swipe_PanGestureRecognizer_Directions_Are_Dynamic_Based_On_Configured_Templates()
    {
        var swipe = new Swipe();

        // Initially no templates are configured -> PanDirection.None (does not intercept any gestures)
        Assert.Equal(PanDirection.None, GetPanDirection(swipe));

        // Configure Left template -> can swipe Right to reveal Left items
        swipe.Left = new DataTemplate();
        Assert.Equal(PanDirection.Right, GetPanDirection(swipe));

        // Configure Right template -> can swipe Right or Left
        swipe.Right = new DataTemplate();
        Assert.Equal(PanDirection.Right | PanDirection.Left, GetPanDirection(swipe));

        // Configure Top template -> can swipe Down to reveal Top items
        swipe.Top = new DataTemplate();
        Assert.Equal(PanDirection.Right | PanDirection.Left | PanDirection.Down, GetPanDirection(swipe));

        // Configure Bottom template -> can swipe Up to reveal Bottom items
        swipe.Bottom = new DataTemplate();
        Assert.Equal(
            PanDirection.Right | PanDirection.Left | PanDirection.Down | PanDirection.Up,
            GetPanDirection(swipe));

        // Remove Left template -> Right, Down, Up
        swipe.Left = null;
        Assert.Equal(
            PanDirection.Left | PanDirection.Down | PanDirection.Up,
            GetPanDirection(swipe));
    }

    [AvaloniaFact]
    public void Swipe_PanGestureRecognizer_Directions_Are_Dynamic_Based_On_SwipeState()
    {
        var swipe = new Swipe
        {
            Left = new DataTemplate(),
            Right = new DataTemplate(),
            Top = new DataTemplate(),
            Bottom = new DataTemplate()
        };

        // When Hidden -> all 4 open directions
        Assert.Equal(
            PanDirection.Right | PanDirection.Left | PanDirection.Down | PanDirection.Up,
            GetPanDirection(swipe));

        // When LeftVisible -> can only swipe Left (to close)
        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(PanDirection.Left, GetPanDirection(swipe));

        // When RightVisible -> can only swipe Right (to close)
        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(PanDirection.Right, GetPanDirection(swipe));

        // When TopVisible -> can only swipe Up (to close)
        swipe.SwipeState = SwipeState.TopVisible;
        Assert.Equal(PanDirection.Up, GetPanDirection(swipe));

        // When BottomVisible -> can only swipe Down (to close)
        swipe.SwipeState = SwipeState.BottomVisible;
        Assert.Equal(PanDirection.Down, GetPanDirection(swipe));

        // Back to Hidden -> all 4 open directions restored
        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(
            PanDirection.Right | PanDirection.Left | PanDirection.Down | PanDirection.Up,
            GetPanDirection(swipe));
    }

    [AvaloniaFact]
    public void Swipe_PanGestureRecognizer_Disabled_When_IsSwipeEnabled_Is_False()
    {
        var swipe = new Swipe
        {
            Left = new DataTemplate(),
            Right = new DataTemplate()
        };

        Assert.Equal(PanDirection.Right | PanDirection.Left, GetPanDirection(swipe));

        swipe.IsSwipeEnabled = false;
        Assert.Equal(PanDirection.None, GetPanDirection(swipe));

        swipe.IsSwipeEnabled = true;
        Assert.Equal(PanDirection.Right | PanDirection.Left, GetPanDirection(swipe));
    }

    // Regression tests for #120: programmatic SwipeState must reveal never-displayed side content.
    [AvaloniaTheory]
    [InlineData(SwipeState.LeftVisible, 70.0, 0.0)]
    [InlineData(SwipeState.RightVisible, -80.0, 0.0)]
    [InlineData(SwipeState.TopVisible, 0.0, 40.0)]
    [InlineData(SwipeState.BottomVisible, 0.0, -50.0)]
    public void Programmatic_SwipeState_Reveals_Content_That_Was_Never_Displayed(SwipeState state, double expectedX, double expectedY)
    {
        var (swipe, window) = CreateSwipeInWindow();

        swipe.SwipeState = state;
        PumpLayout();

        Assert.Equal((expectedX, expectedY), GetCurrentTranslate(swipe));
        Assert.Equal(state, swipe.SwipeState);
        window.Close();
    }

    [AvaloniaTheory]
    [InlineData(SwipeState.LeftVisible, 70.0, 0.0)]
    [InlineData(SwipeState.RightVisible, -80.0, 0.0)]
    public void SwipeState_Set_Before_First_Layout_Is_Applied_After_Layout(SwipeState state, double expectedX, double expectedY)
    {
        var (swipe, window) = CreateSwipeInWindow(show: false);

        swipe.SwipeState = state;
        // Let any queued dispatcher work run before the control gets its first layout pass.
        Dispatcher.UIThread.RunJobs();
        window.Show();
        PumpLayout();

        Assert.Equal((expectedX, expectedY), GetCurrentTranslate(swipe));
        window.Close();
    }

    [AvaloniaFact]
    public void SwipeState_Set_While_Parent_Is_Hidden_Is_Applied_When_Shown()
    {
        var (swipe, window) = CreateSwipeInWindow(show: false);
        window.Content = null;
        var host = new Border { IsVisible = false, Child = swipe };
        window.Content = host;
        window.Show();
        PumpLayout();

        swipe.SwipeState = SwipeState.RightVisible;
        PumpLayout();

        host.IsVisible = true;
        PumpLayout();

        Assert.Equal((-80.0, 0.0), GetCurrentTranslate(swipe));
        window.Close();
    }

    [AvaloniaFact]
    public void Pending_Reveal_Is_Discarded_When_SwipeState_Returns_To_Hidden()
    {
        var (swipe, window) = CreateSwipeInWindow();

        swipe.SwipeState = SwipeState.RightVisible;
        swipe.SwipeState = SwipeState.Hidden;
        PumpLayout();

        Assert.Equal((0.0, 0.0), GetCurrentTranslate(swipe));
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
        window.Close();
    }

    private static (Swipe Swipe, Window Window) CreateSwipeInWindow(bool show = true)
    {
        var swipe = new Swipe
        {
            Width = 300,
            Height = 200,
            AnimationDuration = TimeSpan.Zero,
            Content = new Border { Background = Avalonia.Media.Brushes.Red },
            Left = new FuncDataTemplate<object?>((_, _) => new Border { Width = 70 }),
            Right = new FuncDataTemplate<object?>((_, _) => new Border { Width = 80 }),
            Top = new FuncDataTemplate<object?>((_, _) => new Border { Height = 40 }),
            Bottom = new FuncDataTemplate<object?>((_, _) => new Border { Height = 50 }),
            DataContext = new object()
        };

        var window = new Window { Width = 400, Height = 400, Content = swipe };
        if (show)
        {
            window.Show();
            PumpLayout();
        }

        return (swipe, window);
    }

    private static void PumpLayout()
    {
        Dispatcher.UIThread.RunJobs();
        Avalonia.Headless.AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static (double X, double Y) GetCurrentTranslate(Swipe swipe)
    {
        var x = (double)typeof(Swipe).GetField("_currentX", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(swipe)!;
        var y = (double)typeof(Swipe).GetField("_currentY", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(swipe)!;
        return (x, y);
    }
}



