using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels;

public class CompositionViewModel : ViewModelBase
{
    static CompositionViewModel()
    {
        ViewLocator.Register(typeof(CompositionViewModel), () => new CompositionView());
    }

    public CompositionViewModel()
    {
        Title = "Composition";
    }
}
