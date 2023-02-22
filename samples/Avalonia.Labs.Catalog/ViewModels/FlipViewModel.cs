using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class FlipViewModel : ViewModelBase
    {
        static FlipViewModel()
        {
            ViewLocator.Register(typeof(FlipViewModel), () => new FlipView());
        }

        public FlipViewModel()
        {
            Title = "Flip View";
        }
    }
}
