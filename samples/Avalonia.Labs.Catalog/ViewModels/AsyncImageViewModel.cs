using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class AsyncImageViewModel : ViewModelBase
    {
        static AsyncImageViewModel()
        {
            ViewLocator.Register(typeof(AsyncImageViewModel), () => new AsyncImageView());
        }

        public AsyncImageViewModel()
        {
            Title = "Async Image";
        }
    }
}
