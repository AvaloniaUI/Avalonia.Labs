using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Platform.Storage;

namespace Avalonia.Labs.Catalog.ViewModels;

public class RouteCommandViewModel : ViewModelBase, IViewBinder
{
    static RouteCommandViewModel()
    {
        ViewLocator.Register(typeof(RouteCommandViewModel), () => new RouteCommandView());
    }

    public Visual? View { get; set; }

    public bool CanOpen(object? parameter)
    {
        if (TopLevel.GetTopLevel(View)?.StorageProvider is { CanOpen: true } && parameter is RouteCommandItemViewModel item && item.HasChanges == false)
        {
            return true;
        }
        return false;
    }

    public async void Open(object? parameter)
    {
        if (TopLevel.GetTopLevel(View)?.StorageProvider is { CanOpen: true } provider)
        {
            var result = await provider.OpenFilePickerAsync(new FilePickerOpenOptions() { });

            if (result?.FirstOrDefault() is IStorageFile file)
            {
                if (parameter is RouteCommandItemViewModel item)
                {
                    item.Text = file.Name;
                }
            }
        }
    }

    public bool CanSave(object? parameter) =>
        parameter is RouteCommandItemViewModel { HasChanges: true };

    public async void Save(object? parameter)
    {
        if (parameter is RouteCommandItemViewModel { HasChanges: true } item)
        {
            item.Accept();
        }
        await Task.CompletedTask;
    }

    public IReadOnlyList<RouteCommandItemViewModel> Dettails { get; } =
        new RouteCommandItemViewModel[]
        {
            new() {Id = 1},
            new() {Id = 2},
            new() {Id = 3},
        };
}
