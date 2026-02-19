using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        bool darkTheme = false;

        static MainViewModel()
        {
            ViewLocator.Register(typeof(MainViewModel), () => new MainView());
        }

        public MainViewModel()
        {
            NavigationRouter = new StackNavigationRouter();

            NavigationRouter.NavigateToAsync(new WelcomeViewModel(NavigationRouter), NavigationMode.Clear);
        }

        [ObservableProperty]
        public partial bool? ShowNavBar { get; set; } = true;

        [ObservableProperty]
        public partial bool? ShowBackButton { get; set; } = true;

        [ObservableProperty]
        public partial INavigationRouter NavigationRouter { get; set; }

        public async void NavigateTo(object page)
        {
            if (NavigationRouter != null)
            {
                await NavigationRouter.NavigateToAsync(page);
            }
        }

        public void SwapTheme(object parameter)
        {
            darkTheme = !darkTheme;
            if (darkTheme)
            {
                App.Current!.RequestedThemeVariant = ThemeVariant.Dark;
            }
            else
            {
                App.Current!.RequestedThemeVariant = ThemeVariant.Light;
            }
        }
    }
}
