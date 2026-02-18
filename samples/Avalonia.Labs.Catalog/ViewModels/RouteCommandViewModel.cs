using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Input;
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
        if (TopLevel.GetTopLevel(View)?.StorageProvider is { CanOpen: true } && parameter is RouteCommandItemViewModel
            {
                HasChanges: false
            } or null)
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
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public bool CanSave(object? parameter) =>
        parameter is RouteCommandItemViewModel { HasChanges: true };

    public async void Save(object? parameter)
    {
        if (parameter is RouteCommandItemViewModel { HasChanges: true } item)
        {
            item.Accept();
            CommandManager.InvalidateRequerySuggested();
        }
        await Task.CompletedTask;
    }

    public bool CanDelete(object? parameter) =>
        parameter is RouteCommandItemViewModel;

    public void Delete(object? parameter)
    {
        if (parameter is RouteCommandItemViewModel item)
        {
            Dettails.Remove(item);
            item.Accept();
            CommandManager.InvalidateRequerySuggested();
        }
    }
    
    public ObservableCollection<RouteCommandItemViewModel> Dettails { get; } =
        new ObservableCollection<RouteCommandItemViewModel>()
        {
            new() {Id = 1},
            new() {Id = 2},
            new() {Id = 3},
        };
}
