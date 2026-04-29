using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Xunit;

namespace Avalonia.Labs.Controls.Tests;

public class SwipeTests
{
    [Fact]
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

    [Fact]
    public void Swipe_Has_Default_Threshold_Of_100()
    {
        var swipe = new Swipe();

        Assert.Equal(100.0, swipe.Threshold);
    }

    [Fact]
    public void Swipe_Has_Default_AnimationDuration_Of_200ms()
    {
        var swipe = new Swipe();

        Assert.Equal(TimeSpan.FromMilliseconds(200), swipe.AnimationDuration);
    }

    [Fact]
    public void Swipe_Has_Default_SwipeModes_As_Reveal()
    {
        var swipe = new Swipe();

        Assert.Equal(SwipeMode.Reveal, swipe.LeftMode);
        Assert.Equal(SwipeMode.Reveal, swipe.RightMode);
        Assert.Equal(SwipeMode.Reveal, swipe.TopMode);
        Assert.Equal(SwipeMode.Reveal, swipe.BottomMode);
    }

    [Fact]
    public void Swipe_ClipToBounds_Is_True()
    {
        var swipe = new Swipe();

        Assert.True(swipe.ClipToBounds);
    }

    [Fact]
    public void Swipe_Is_Focusable()
    {
        var swipe = new Swipe();

        Assert.True(swipe.Focusable);
    }

    [Fact]
    public void Swipe_Can_Set_Left_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Left = template;

        Assert.Equal(template, swipe.Left);
    }

    [Fact]
    public void Swipe_Can_Set_Right_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Right = template;

        Assert.Equal(template, swipe.Right);
    }

    [Fact]
    public void Swipe_Can_Set_Top_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Top = template;

        Assert.Equal(template, swipe.Top);
    }

    [Fact]
    public void Swipe_Can_Set_Bottom_Template()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Bottom = template;

        Assert.Equal(template, swipe.Bottom);
    }

    [Fact]
    public void Swipe_Can_Set_Content()
    {
        var swipe = new Swipe();
        var content = new TextBlock { Text = "Test" };

        swipe.Content = content;

        Assert.Equal(content, swipe.Content);
    }

    [Fact]
    public void Swipe_Can_Clear_Templates()
    {
        var swipe = new Swipe();
        var template = new DataTemplate();

        swipe.Left = template;
        swipe.Left = null;

        Assert.Null(swipe.Left);
    }

    [Fact]
    public void Swipe_Can_Set_Threshold()
    {
        var swipe = new Swipe();

        swipe.Threshold = 150.0;

        Assert.Equal(150.0, swipe.Threshold);
    }

    [Fact]
    public void Swipe_Threshold_Can_Be_Zero()
    {
        var swipe = new Swipe();

        swipe.Threshold = 0;

        Assert.Equal(0, swipe.Threshold);
    }

    [Fact]
    public void Swipe_Threshold_Can_Be_Negative()
    {
        var swipe = new Swipe();

        swipe.Threshold = -50.0;

        Assert.Equal(-50.0, swipe.Threshold);
    }

    [Fact]
    public void Swipe_Can_Set_AnimationDuration()
    {
        var swipe = new Swipe();

        swipe.AnimationDuration = TimeSpan.FromMilliseconds(500);

        Assert.Equal(TimeSpan.FromMilliseconds(500), swipe.AnimationDuration);
    }

    [Fact]
    public void Swipe_AnimationDuration_Can_Be_Zero()
    {
        var swipe = new Swipe();

        swipe.AnimationDuration = TimeSpan.Zero;

        Assert.Equal(TimeSpan.Zero, swipe.AnimationDuration);
    }

    [Fact]
    public void Swipe_Can_Set_LeftMode()
    {
        var swipe = new Swipe();

        swipe.LeftMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.LeftMode);
    }

    [Fact]
    public void Swipe_Can_Set_RightMode()
    {
        var swipe = new Swipe();

        swipe.RightMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.RightMode);
    }

    [Fact]
    public void Swipe_Can_Set_TopMode()
    {
        var swipe = new Swipe();

        swipe.TopMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.TopMode);
    }

    [Fact]
    public void Swipe_Can_Set_BottomMode()
    {
        var swipe = new Swipe();

        swipe.BottomMode = SwipeMode.Execute;

        Assert.Equal(SwipeMode.Execute, swipe.BottomMode);
    }

    [Fact]
    public void SwipeMode_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<SwipeMode>();

        Assert.Contains(SwipeMode.Reveal, values);
        Assert.Contains(SwipeMode.Execute, values);
        Assert.Equal(2, values.Length);
    }

    [Fact]
    public void SwipeState_Can_Be_Set_To_LeftVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;

        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
    }

    [Fact]
    public void SwipeState_Can_Be_Set_To_RightVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.RightVisible;

        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
    }

    [Fact]
    public void SwipeState_Can_Be_Set_To_TopVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.TopVisible;

        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);
    }

    [Fact]
    public void SwipeState_Can_Be_Set_To_BottomVisible()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.BottomVisible;

        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);
    }

    [Fact]
    public void SwipeState_Can_Be_Set_To_Hidden()
    {
        var swipe = new Swipe();
        swipe.SwipeState = SwipeState.LeftVisible;

        swipe.SwipeState = SwipeState.Hidden;

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [Fact]
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

    [Fact]
    public void SwipeState_Hidden_Is_Default_Value()
    {
        Assert.Equal(0, (int)SwipeState.Hidden);
    }

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
    public void Setting_SwipeState_To_Current_Value_Does_Not_Throw()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.Hidden;
        swipe.SwipeState = SwipeState.Hidden;

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [Fact]
    public void LeftTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.LeftTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Left", property.Name);
    }

    [Fact]
    public void RightTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.RightTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Right", property.Name);
    }

    [Fact]
    public void TopTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.TopTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Top", property.Name);
    }

    [Fact]
    public void BottomTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.BottomTemplateProperty;

        Assert.NotNull(property);
        Assert.Equal("Bottom", property.Name);
    }

    [Fact]
    public void ContentProperty_Is_StyledProperty()
    {
        var property = Swipe.ContentProperty;

        Assert.NotNull(property);
        Assert.Equal("Content", property.Name);
    }

    [Fact]
    public void SwipeStateProperty_Is_StyledProperty()
    {
        var property = Swipe.SwipeStateProperty;

        Assert.NotNull(property);
        Assert.Equal("SwipeState", property.Name);
    }

    [Fact]
    public void ThresholdProperty_Is_StyledProperty()
    {
        var property = Swipe.ThresholdProperty;

        Assert.NotNull(property);
        Assert.Equal("Threshold", property.Name);
    }

    [Fact]
    public void AnimationDurationProperty_Is_StyledProperty()
    {
        var property = Swipe.AnimationDurationProperty;

        Assert.NotNull(property);
        Assert.Equal("AnimationDuration", property.Name);
    }

    [Fact]
    public void LeftModeProperty_Is_StyledProperty()
    {
        var property = Swipe.LeftModeProperty;

        Assert.NotNull(property);
        Assert.Equal("LeftMode", property.Name);
    }

    [Fact]
    public void RightModeProperty_Is_StyledProperty()
    {
        var property = Swipe.RightModeProperty;

        Assert.NotNull(property);
        Assert.Equal("RightMode", property.Name);
    }

    [Fact]
    public void TopModeProperty_Is_StyledProperty()
    {
        var property = Swipe.TopModeProperty;

        Assert.NotNull(property);
        Assert.Equal("TopMode", property.Name);
    }

    [Fact]
    public void BottomModeProperty_Is_StyledProperty()
    {
        var property = Swipe.BottomModeProperty;

        Assert.NotNull(property);
        Assert.Equal("BottomMode", property.Name);
    }

    [Fact]
    public void OpenSwipeItem_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<OpenSwipeItem>();

        Assert.Contains(OpenSwipeItem.LeftItems, values);
        Assert.Contains(OpenSwipeItem.RightItems, values);
        Assert.Contains(OpenSwipeItem.TopItems, values);
        Assert.Contains(OpenSwipeItem.BottomItems, values);
        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void SwipeDirection_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<SwipeDirection>();

        Assert.Contains(SwipeDirection.Left, values);
        Assert.Contains(SwipeDirection.Right, values);
        Assert.Contains(SwipeDirection.Up, values);
        Assert.Contains(SwipeDirection.Down, values);
        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void OpenRequestedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.OpenRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("OpenRequested", eventInfo.Name);
    }

    [Fact]
    public void CloseRequestedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.CloseRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("CloseRequested", eventInfo.Name);
    }

    [Fact]
    public void SwipeStartedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.SwipeStartedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("SwipeStarted", eventInfo.Name);
    }

    [Fact]
    public void SwipeEndedEvent_Is_Routed_Event()
    {
        var eventInfo = Swipe.SwipeEndedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal("SwipeEnded", eventInfo.Name);
    }

    [Fact]
    public void OpenRequestedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.OpenRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [Fact]
    public void CloseRequestedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.CloseRequestedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [Fact]
    public void SwipeStartedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.SwipeStartedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [Fact]
    public void SwipeEndedEvent_Is_Bubble_Routing()
    {
        var eventInfo = Swipe.SwipeEndedEvent;

        Assert.NotNull(eventInfo);
        Assert.Equal(RoutingStrategies.Bubble, eventInfo.RoutingStrategies);
    }

    [Fact]
    public void OpenRequestedEventArgs_Has_Cancel_Property()
    {
        var args = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.LeftItems);

        Assert.False(args.Cancel);
        args.Cancel = true;
        Assert.True(args.Cancel);
    }

    [Fact]
    public void OpenRequestedEventArgs_Has_OpenSwipeItem_Property()
    {
        var args = new OpenRequestedEventArgs(Swipe.OpenRequestedEvent, OpenSwipeItem.RightItems);

        Assert.Equal(OpenSwipeItem.RightItems, args.OpenSwipeItem);
    }

    [Fact]
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

    [Fact]
    public void CloseRequestedEventArgs_Has_Cancel_Property()
    {
        var args = new CloseRequestedEventArgs(Swipe.CloseRequestedEvent);

        Assert.False(args.Cancel);
        args.Cancel = true;
        Assert.True(args.Cancel);
    }

    [Fact]
    public void SwipeStartedEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeStartedEventArgs(Swipe.SwipeStartedEvent, SwipeDirection.Left);

        Assert.Equal(SwipeDirection.Left, args.SwipeDirection);
    }

    [Fact]
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

    [Fact]
    public void SwipeEndedEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Right, true);

        Assert.Equal(SwipeDirection.Right, args.SwipeDirection);
    }

    [Fact]
    public void SwipeEndedEventArgs_Has_IsOpen_Property()
    {
        var argsOpen = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Left, true);
        var argsClosed = new SwipeEndedEventArgs(Swipe.SwipeEndedEvent, SwipeDirection.Left, false);

        Assert.True(argsOpen.IsOpen);
        Assert.False(argsClosed.IsOpen);
    }

    [Fact]
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

    [Fact]
    public void SwipeChangingEventArgs_Has_SwipeDirection_Property()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Up, 50.0);

        Assert.Equal(SwipeDirection.Up, args.SwipeDirection);
    }

    [Fact]
    public void SwipeChangingEventArgs_Has_Offset_Property()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Down, 75.5);

        Assert.Equal(75.5, args.Offset);
    }

    [Fact]
    public void SwipeChangingEventArgs_SwipeDirection_Is_Mutable()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, 0);

        args.SwipeDirection = SwipeDirection.Right;

        Assert.Equal(SwipeDirection.Right, args.SwipeDirection);
    }

    [Fact]
    public void SwipeChangingEventArgs_Offset_Is_Mutable()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, 50.0);

        args.Offset = 100.0;

        Assert.Equal(100.0, args.Offset);
    }

    [Fact]
    public void SwipeChangingEventArgs_Offset_Can_Be_Negative()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Left, -50.0);

        Assert.Equal(-50.0, args.Offset);
    }

    [Fact]
    public void SwipeChangingEventArgs_Offset_Can_Be_Zero()
    {
        var args = new SwipeChangingEventArgs(SwipeDirection.Right, 0);

        Assert.Equal(0, args.Offset);
    }

    [Fact]
    public void Swipe_Inherits_DataContext()
    {
        var swipe = new Swipe();
        var dataContext = new { Name = "Test" };

        swipe.DataContext = dataContext;

        Assert.Same(dataContext, swipe.DataContext);
    }

    [Fact]
    public void Swipe_DataContext_Can_Be_Null()
    {
        var swipe = new Swipe();
        swipe.DataContext = new { Name = "Test" };

        swipe.DataContext = null;

        Assert.Null(swipe.DataContext);
    }

    [Fact]
    public void Swipe_Has_Five_Children_After_Construction()
    {
        var swipe = new Swipe();

        // Should have 4 side containers + 1 body container
        Assert.Equal(5, swipe.Children.Count);
    }

    [Fact]
    public void Swipe_Inherits_From_Grid()
    {
        var swipe = new Swipe();

        Assert.IsAssignableFrom<Grid>(swipe);
    }

    [Fact]
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

    [Fact]
    public void ThresholdProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.ThresholdProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(100.0, defaultValue);
    }

    [Fact]
    public void AnimationDurationProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.AnimationDurationProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(TimeSpan.FromMilliseconds(200), defaultValue);
    }

    [Fact]
    public void LeftModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.LeftModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [Fact]
    public void RightModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.RightModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [Fact]
    public void TopModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.TopModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }

    [Fact]
    public void BottomModeProperty_Has_Correct_Default_Value()
    {
        var defaultValue = Swipe.BottomModeProperty.GetDefaultValue(typeof(Swipe));

        Assert.Equal(SwipeMode.Reveal, defaultValue);
    }
}
