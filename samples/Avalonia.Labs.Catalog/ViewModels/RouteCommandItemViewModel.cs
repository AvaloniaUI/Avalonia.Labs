using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels;

public partial class RouteCommandItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string? Text { get; set; }

    public int Id { get; internal set; }

    [ObservableProperty]
    public partial bool HasChanges { get; set; }

    public void Accept()
    {
        HasChanges = false;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Text))
        {
            HasChanges = true;
        }
    }
}
