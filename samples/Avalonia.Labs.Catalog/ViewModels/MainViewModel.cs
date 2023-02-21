using Avalonia.Labs.Controls;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool? _showNavBar = true;
        private bool? _showBackButton = true;
        private INavigationRouter _navigationRouter;
        private string? _title;

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

        public string? Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
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
    }
}