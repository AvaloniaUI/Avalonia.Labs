using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Media;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class GifViewModel : ViewModelBase
    {
        static GifViewModel()
        {
            ViewLocator.Register(typeof(GifViewModel), () => new GifView());
        }

        public GifViewModel()
        {
            Title = "GIF";
            var assets = AssetLoader
                .GetAssets(new Uri("avares://Avalonia.Labs.Catalog/Assets/Gifs"),
                    new Uri("avares://Avalonia.Labs.Catalog/"))
                .Where(x => x.AbsolutePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .Select(x => new AssetModel(Path.GetFileName(x.AbsoluteUri), x.AbsoluteUri));

            _assets = assets is not null ? new ObservableCollection<AssetModel>(assets) : new();

            Stretches = new List<Stretch>
            {
                Stretch.None,
                Stretch.Fill,
                Stretch.Uniform,
                Stretch.UniformToFill
            };

            SelectedStretch = Stretch.None;

            SelectedAsset = _assets.FirstOrDefault();
        }

        private readonly ObservableCollection<AssetModel> _assets;

        public IReadOnlyList<AssetModel> Assets => _assets;

        [ObservableProperty]
        public partial AssetModel? SelectedAsset { get; set; }

        [ObservableProperty]
        public partial bool EnableCheckerboard { get; set; }


        [ObservableProperty]
        public partial int IterationCount { get; set; }

        [ObservableProperty]
        public partial IReadOnlyList<Stretch>? Stretches { get; set; }

        [ObservableProperty]
        public partial Stretch? SelectedStretch { get; set; }

        public void Add(string path)
        {
            _assets.Add(new AssetModel(Path.GetFileName(path), path));
        }

        public void Previous()
        {
            if (SelectedAsset is { } && _assets.Count > 1)
            {
                var index = _assets.IndexOf(SelectedAsset);
                if (index == 0)
                {
                    SelectedAsset = _assets[_assets.Count - 1];
                }
                else if (index > 0)
                {
                    SelectedAsset = _assets[index - 1];
                }
            }
        }

        public void Next()
        {
            if (SelectedAsset is { } && _assets.Count > 1)
            {
                var index = _assets.IndexOf(SelectedAsset);
                if (index == _assets.Count - 1)
                {
                    SelectedAsset = _assets[0];
                }
                else if (index >= 0 && index < _assets.Count - 1)
                {
                    SelectedAsset = _assets[index + 1];
                }
            }
        }
    }
}
