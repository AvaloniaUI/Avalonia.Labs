using Avalonia.Controls;
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
        Assert.Null(swipe.Content);
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
    public void SwipeState_Can_Be_Changed()
    {
        var swipe = new Swipe();

        swipe.SwipeState = SwipeState.LeftVisible;
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.RightVisible;
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.SwipeState = SwipeState.Hidden;
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [Fact]
    public void SwipeState_Enum_Has_Correct_Values()
    {
        var values = Enum.GetValues<SwipeState>();

        Assert.Contains(SwipeState.Hidden, values);
        Assert.Contains(SwipeState.LeftVisible, values);
        Assert.Contains(SwipeState.RightVisible, values);
    }

    [Fact]
    public void LeftTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.LeftTemplateProperty;

        Assert.NotNull(property);
        Assert.IsAssignableFrom<StyledProperty<DataTemplate>>(property);
    }

    [Fact]
    public void RightTemplateProperty_Is_StyledProperty()
    {
        var property = Swipe.RightTemplateProperty;

        Assert.NotNull(property);
        Assert.IsAssignableFrom<StyledProperty<DataTemplate>>(property);
    }

    [Fact]
    public void ContentProperty_Is_StyledProperty()
    {
        var property = Swipe.ContentProperty;

        Assert.NotNull(property);
        Assert.IsAssignableFrom<StyledProperty<Control>>(property);
    }

    [Fact]
    public void SwipeStateProperty_Is_StyledProperty()
    {
        var property = Swipe.SwipeStateProperty;

        Assert.NotNull(property);
        Assert.IsAssignableFrom<StyledProperty<SwipeState>>(property);
    }

    [Fact]
    public void Open_Sets_SwipeState_To_LeftVisible()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.LeftItems);

        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);
    }

    [Fact]
    public void Open_Sets_SwipeState_To_RightVisible()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.RightItems);

        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
    }

    [Fact]
    public void Open_Sets_SwipeState_To_TopVisible()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.TopItems);

        Assert.Equal(SwipeState.TopVisible, swipe.SwipeState);
    }

    [Fact]
    public void Open_Sets_SwipeState_To_BottomVisible()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.BottomItems);

        Assert.Equal(SwipeState.BottomVisible, swipe.SwipeState);
    }

    [Fact]
    public void Close_Sets_SwipeState_To_Hidden()
    {
        var swipe = new Swipe();
        swipe.SwipeState = SwipeState.LeftVisible;

        swipe.Close();

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [Fact]
    public void Open_With_Animation_False_Opens_Without_Animation()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.RightItems, animated: false);

        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);
    }

    [Fact]
    public void Close_With_Animation_False_Closes_Without_Animation()
    {
        var swipe = new Swipe();
        swipe.SwipeState = SwipeState.RightVisible;

        swipe.Close(animated: false);

        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }

    [Fact]
    public void OpenSwipeItem_Enum_Has_Correct_Values()
    {
        var values = System.Enum.GetValues<OpenSwipeItem>();

        Assert.Contains(OpenSwipeItem.LeftItems, values);
        Assert.Contains(OpenSwipeItem.RightItems, values);
        Assert.Contains(OpenSwipeItem.TopItems, values);
        Assert.Contains(OpenSwipeItem.BottomItems, values);
    }

    [Fact]
    public void Multiple_Open_Close_Cycles_Work_Correctly()
    {
        var swipe = new Swipe();

        swipe.Open(OpenSwipeItem.LeftItems);
        Assert.Equal(SwipeState.LeftVisible, swipe.SwipeState);

        swipe.Close();
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);

        swipe.Open(OpenSwipeItem.RightItems);
        Assert.Equal(SwipeState.RightVisible, swipe.SwipeState);

        swipe.Close();
        Assert.Equal(SwipeState.Hidden, swipe.SwipeState);
    }
}
