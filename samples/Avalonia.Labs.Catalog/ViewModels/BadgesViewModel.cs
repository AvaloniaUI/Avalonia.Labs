using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels;

internal class BadgesViewModel:ViewModelBase
{
    static BadgesViewModel() 
    {
        ViewLocator.Register(typeof(BadgesViewModel), () => new BadgesView());
    } 
}
