using System.Linq;
using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels;

public class SwipeViewModel : ViewModelBase
{
    static SwipeViewModel()
    {
        ViewLocator.Register(typeof(SwipeViewModel), () => new SwipeView());
    }

    public SwipeViewModel()
    {
        Title = "Swipe";
        Items = Enumerable.Range(0, 20).ToArray();
    }

    public int[] Items
    {
        get;
        set;
    }
}
