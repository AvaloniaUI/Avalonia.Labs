using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels;

public class ViewModelBase : ReactiveObject
{
    private string? _title;

    public string? Title
    {
        get => _title; 
        set
        {
            this.RaiseAndSetIfChanged(ref _title, value);
        }
    }
}
