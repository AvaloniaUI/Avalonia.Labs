using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Labs.Controls.Tests;

public class ContentDialogTests
{
    [AvaloniaFact]
    public void IsLightDismissEnabled_Is_False_By_Default()
    {
        Assert.False(new ContentDialog().IsLightDismissEnabled);
    }

    [AvaloniaFact]
    public void Pressing_Background_Closes_Dialog_With_None_When_Light_Dismiss_Is_Enabled()
    {
        var window = CreateWindow();
        var (_, result) = ShowDialog(window, lightDismiss: true);

        window.MouseDown(new Point(5, 5), MouseButton.Left);
        window.MouseUp(new Point(5, 5), MouseButton.Left);
        PumpUntil(() => result.IsCompleted);

        Assert.True(result.IsCompleted);
        Assert.Equal(ContentDialogResult.None, result.Result);
        window.Close();
    }

    [AvaloniaFact]
    public void Pressing_Background_Does_Not_Close_Dialog_When_Light_Dismiss_Is_Disabled()
    {
        var window = CreateWindow();
        var (dialog, result) = ShowDialog(window, lightDismiss: false);

        window.MouseDown(new Point(5, 5), MouseButton.Left);
        window.MouseUp(new Point(5, 5), MouseButton.Left);
        PumpUntil(() => result.IsCompleted, 500);

        Assert.False(result.IsCompleted);
        dialog.Hide();
        PumpUntil(() => result.IsCompleted);
        window.Close();
    }

    [AvaloniaFact]
    public void Pressing_Inside_Dialog_Does_Not_Close_Dialog()
    {
        var window = CreateWindow();
        var (dialog, result) = ShowDialog(window, lightDismiss: true);

        // Center of the window is inside the dialog box.
        window.MouseDown(new Point(400, 300), MouseButton.Left);
        window.MouseUp(new Point(400, 300), MouseButton.Left);
        PumpUntil(() => result.IsCompleted, 500);

        Assert.False(result.IsCompleted);
        dialog.Hide();
        PumpUntil(() => result.IsCompleted);
        window.Close();
    }

    [AvaloniaFact]
    public void Light_Dismiss_Can_Be_Cancelled_From_Closing()
    {
        var window = CreateWindow();
        var (dialog, result) = ShowDialog(window, lightDismiss: true);
        var closingCount = 0;
        dialog.Closing += (_, e) =>
        {
            closingCount++;
            e.Cancel = true;
        };

        window.MouseDown(new Point(5, 5), MouseButton.Left);
        window.MouseUp(new Point(5, 5), MouseButton.Left);
        PumpUntil(() => result.IsCompleted, 500);

        Assert.Equal(1, closingCount);
        Assert.False(result.IsCompleted);
        window.Close();
    }

    private static Window CreateWindow()
    {
        var window = new Window { Width = 800, Height = 600 };
        window.Styles.Add(new ControlThemes());
        window.Show();
        return window;
    }

    private static void PumpUntil(Func<bool> condition, int timeoutMs = 5000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Thread.Sleep(10);
        }
    }

    private static (ContentDialog Dialog, Task<ContentDialogResult> Result) ShowDialog(Window window, bool lightDismiss)
    {
        var dialog = new ContentDialog
        {
            Title = "Title",
            Content = "Content",
            PrimaryButtonText = "OK",
            CloseButtonText = "Cancel",
            IsLightDismissEnabled = lightDismiss
        };

        var result = dialog.ShowAsync(window);
        PumpUntil(() => dialog.IsLoaded, 1000);
        Assert.NotNull(dialog.GetVisualDescendants().OfType<Border>().FirstOrDefault(b => b.Name == "BackgroundElement"));
        return (dialog, result);
    }
}
