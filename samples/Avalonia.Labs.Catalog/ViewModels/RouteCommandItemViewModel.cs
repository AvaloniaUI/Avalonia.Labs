using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels;

public class RouteCommandItemViewModel : ViewModelBase
{
    private string? _text;
    private bool _hasChanges;

    public string? Text
    {
        get => _text;
        set
        {
            this.RaiseAndSetIfChanged(ref _text, value);
            HasChanges = true;
        }
    }

    public int Id { get; internal set; }

    public bool HasChanges { get => _hasChanges; private set => this.RaiseAndSetIfChanged(ref _hasChanges, value); }

    public void Accept()
    {
        HasChanges = false;
    }
}
