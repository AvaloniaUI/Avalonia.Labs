using Xunit;
using Avalonia.Labs.Controls;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls;

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
        var values = System.Enum.GetValues<SwipeState>();

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
}
