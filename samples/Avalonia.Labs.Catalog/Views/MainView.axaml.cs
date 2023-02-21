using Avalonia.Controls;
using Avalonia.Labs.Catalog.ViewModels;

namespace Avalonia.Labs.Catalog.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel != null)
        {
            topLevel.BackRequested += TopLevel_BackRequested;
        }
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

    protected override void OnUnloaded()
    {
        base.OnUnloaded();

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel != null)
        {
            topLevel.BackRequested -= TopLevel_BackRequested;
        }
    }
}