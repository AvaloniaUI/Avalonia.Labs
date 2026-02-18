using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    public partial string? Title { get; set; }
}
