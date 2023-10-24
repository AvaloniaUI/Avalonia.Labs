using System.Threading.Tasks;
using Avalonia.Labs.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using ReactiveUI;
using SkiaSharp;

namespace Avalonia.Labs.Catalog.ViewModels;

internal class SKXamlCanvasViewModel : ViewModelBase
{
    private SKBitmap? _bitmap;

    static SKXamlCanvasViewModel()
    {
        ViewLocator.Register(typeof(SKXamlCanvasViewModel), () => new SKXamlCanvasView());
    }

    public SKXamlCanvasViewModel()
    {
        Title = nameof(SKCanvasView);
    }

    public SKBitmap? Bitmap { get => _bitmap; set => this.RaiseAndSetIfChanged(ref _bitmap, value); }

    public async void OpenAsync(object? parameter)
    {
        if (Services.FsSevice.GetStorageProvider() is IStorageProvider { CanOpen: true } storage)
        {
            var result = await storage.OpenFilePickerAsync(new()
            {
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
            });
            if (result is { Count: > 0 } && result[0] is IStorageFile f)
            {
                var stream = await f.OpenReadAsync();
                Bitmap = SKBitmap.Decode(stream);
            }
        }
    }

    public async void ApplyAsync(object? parameter)
    {
        if ((parameter as TabItem)?.FindDescendantOfType<Controls.BindableCanvas>() is { } canvas)
        {
            Bitmap = canvas.Result;
        }
        await Task.CompletedTask;
    }

}
