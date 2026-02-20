using System.Collections.Generic;
using Avalonia.Labs.Catalog.Views;

namespace Avalonia.Labs.Catalog.ViewModels;

public class MultiSelectionComboBoxViewModel : ViewModelBase
{
    static MultiSelectionComboBoxViewModel()
    {
        ViewLocator.Register(typeof(MultiSelectionComboBoxViewModel), () => new MultiSelectionComboBoxView());
    }
    
    public List<string> Items { get; } =
    [
        "Hello", "Avalonia"
    ];
}
