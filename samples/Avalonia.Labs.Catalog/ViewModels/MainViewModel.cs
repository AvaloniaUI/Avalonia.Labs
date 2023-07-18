using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using Avalonia.Styling;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        bool darkTheme = false;

        static MainViewModel()
        {
            ViewLocator.Register(typeof(MainViewModel), () => new MainView());
        }

        private bool? _showNavBar = true;
        private bool? _showBackButton = true;
        private INavigationRouter _navigationRouter;

        public MainViewModel()
        {
            _navigationRouter = new StackNavigationRouter();

            _navigationRouter.NavigateToAsync(new WelcomeViewModel(_navigationRouter), NavigationMode.Clear);
        }

        public bool? ShowNavBar
        {
            get => _showNavBar;
            set => this.RaiseAndSetIfChanged(ref _showNavBar, value);
        }

        public bool? ShowBackButton
        {
            get => _showBackButton;
            set => this.RaiseAndSetIfChanged(ref _showBackButton, value);
        }
        public INavigationRouter NavigationRouter
        {
            get => _navigationRouter;
            set => this.RaiseAndSetIfChanged(ref _navigationRouter, value);
        }

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
