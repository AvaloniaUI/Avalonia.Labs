using System.Linq;
using Avalonia.Labs.Catalog.Views;
namespace Avalonia.Labs.Catalog.ViewModels;

public class GestureViewModel : ViewModelBase
{
    static GestureViewModel()
    {
        ViewLocator.Register(typeof(GestureViewModel), () => new GestureContainerView());
    }

    public GestureViewModel()
    {
        Title = "Gesture";
        Items = Enumerable.Range(0, 10).ToArray();
    }

    public int[] Items
    {
        get;
        set;
    }
}
