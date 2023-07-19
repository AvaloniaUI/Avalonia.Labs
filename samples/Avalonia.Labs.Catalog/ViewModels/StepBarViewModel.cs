using System.Collections.Generic;
using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels;

public record class Step(int Id, string Header, string Description);

internal class StepBarViewModel : ViewModelBase
{
    static StepBarViewModel()
    {
        ViewLocator.Register(typeof(StepBarViewModel), () => new StepBarView());
    }

    public IReadOnlyList<Step> Steps { get; } =
        new Step[]
        {
            new(1,"Step","Registration"),
            new(2,"Step","Fill in basic information"),
            new(3,"Step","Upload file"),
            new(4,"Step","Complete"),
        };
}
