using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Catalog.ViewModels;

namespace Avalonia.Labs.Catalog.Views;

public partial class MainView : UserControl
{
    public static readonly StyledProperty<bool> IsLargeWidthProperty = AvaloniaProperty.Register<MainView, bool>(nameof(IsLargeWidth), true, defaultBindingMode: Data.BindingMode.TwoWay, 
        coerce: (x, y) =>
        {
            return y;
        });

    public bool IsLargeWidth
    {
        get => GetValue(IsLargeWidthProperty);
        set => SetValue(IsLargeWidthProperty, value);
    }

    public MainView()
    {
        InitializeComponent();
        DataContextChanged += MainView_DataContextChanged;
    }

    private void MainView_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.ShouldClosePaneOnNavigate = () => !IsLargeWidth;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel != null)
        {
            topLevel.BackRequested += TopLevel_BackRequested;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    }

    private async void TopLevel_BackRequested(object? sender, Interactivity.RoutedEventArgs e)
    {
        var viewModel = DataContext as MainViewModel;

        if(viewModel != null)
        {
            if(viewModel.NavigationRouter.CanGoBack)
            {
                e.Handled = true;

                await viewModel.NavigationRouter.BackAsync();
            }
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            topLevel.BackRequested -= TopLevel_BackRequested;
        }
    }

    private void MenuButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
    }
}
