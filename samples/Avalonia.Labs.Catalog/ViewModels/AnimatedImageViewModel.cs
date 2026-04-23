using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Labs.AnimatedImage;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Media;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class AnimatedImageViewModel : ViewModelBase
    {
        static AnimatedImageViewModel()
        {
            ViewLocator.Register(typeof(AnimatedImageViewModel), () => new AnimatedImageView());
        }

        public AnimatedImageViewModel()
        {
            Title = "Animated Image";

            var assets = EnumerateAnimatedAssets()
                .Select(x => new AssetModel(Path.GetFileName(x.AbsolutePath), x.AbsoluteUri));

            _assets = assets is not null ? new ObservableCollection<AssetModel>(assets) : new();

            Stretches = new List<Stretch>
            {
                Stretch.None,
                Stretch.Fill,
                Stretch.Uniform,
                Stretch.UniformToFill
            };

            SelectedStretch = Stretch.Uniform;
            IsPlaying = true;
        }

        private readonly ObservableCollection<AssetModel> _assets;

        public IReadOnlyList<AssetModel> Assets => _assets;

        [ObservableProperty]
        public partial AssetModel? SelectedAsset { get; set; }

        [ObservableProperty]
        public partial IAnimatedBitmap? SelectedSource { get; set; }

        [ObservableProperty]
        public partial bool IsPlaying { get; set; }

        [ObservableProperty]
        public partial IReadOnlyList<Stretch>? Stretches { get; set; }

        [ObservableProperty]
        public partial Stretch? SelectedStretch { get; set; }

        partial void OnSelectedAssetChanged(AssetModel? value)
        {
            SelectedSource = value is not null
                ? CreateSource(value.Path)
                : null;
        }

        partial void OnSelectedSourceChanged(IAnimatedBitmap? oldValue, IAnimatedBitmap? newValue)
        {
            oldValue?.Dispose();
        }

        private static IEnumerable<Uri> EnumerateAnimatedAssets()
        {
            var root = new Uri("avares://Avalonia.Labs.Catalog/");
            var gifs = AssetLoader.GetAssets(new Uri("avares://Avalonia.Labs.Catalog/Assets/Gifs"), root);

            var animatedImages = AssetLoader.GetAssets(new Uri("avares://Avalonia.Labs.Catalog/Assets/AnimatedImages"), root);

            return gifs.Concat(animatedImages).OrderBy(x => x.AbsoluteUri);
        }

        private static IAnimatedBitmap? CreateSource(string assetPath)
        {
            if (!Uri.TryCreate(assetPath, UriKind.Absolute, out var assetUri))
            {
                return null;
            }

            var stream = AssetLoader.Open(assetUri);
            return IAnimatedBitmap.Load(stream, disposeStream: true);
        }
    }
}
