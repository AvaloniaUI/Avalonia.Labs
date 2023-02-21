using Avalonia.Labs.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
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
