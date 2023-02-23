using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class TabLayoutViewModel : ViewModelBase
    {
        static TabLayoutViewModel()
        {
            ViewLocator.Register(typeof(TabLayoutViewModel), () => new TabLayoutView());
        }

        public TabLayoutViewModel()
        {
            Title = "Tab Layout";
        }
    }
}
