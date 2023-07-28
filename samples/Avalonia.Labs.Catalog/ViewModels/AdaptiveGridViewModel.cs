using System.Collections.Generic;

namespace Avalonia.Labs.Catalog.ViewModels;

internal class AdaptiveGridViewModel : ViewModelBase
{
    static AdaptiveGridViewModel()
    {
        ViewLocator.Register(typeof(AdaptiveGridViewModel), () => new Views.AdaptiveGridView());
    }

    public AdaptiveGridViewModel()
    {
        Title = "AdaptiveGrid";
    }

    public IReadOnlyList<Step> Steps { get; } =
    new Step[]
    {
            new(1,"Step","Registration"),
            new(2,"Step 2","Fill in basic information"),
            new(3,"Step 3","Upload file"),
            new(4,"Step 4","Complete"),
    };
}
