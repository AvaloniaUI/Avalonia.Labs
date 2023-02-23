using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        static WelcomeViewModel()
        {
            ViewLocator.Register(typeof(WelcomeViewModel), () => new WelcomeView());
        }

        public WelcomeViewModel(INavigationRouter navigationRouter)
        {
            Title = "Welcome";
            NavigationRouter = navigationRouter;
        }

        public INavigationRouter NavigationRouter { get; }

        public async void NavigateTo(object page)
        {
            await NavigationRouter.NavigateToAsync(page);
        }
    }
}
