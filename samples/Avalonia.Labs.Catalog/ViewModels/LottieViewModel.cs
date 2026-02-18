using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class LottieViewModel : ViewModelBase
    {
        static LottieViewModel()
        {
            ViewLocator.Register(typeof(LottieViewModel), () => new LottieView());
        }

        public LottieViewModel()
        {
            Title = "Lottie";
            var assets = AssetLoader
                .GetAssets(new Uri("avares://Avalonia.Labs.Catalog/Assets"), new Uri("avares://Avalonia.Labs.Catalog/"))
                .Where(x => x.AbsolutePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(x => new AssetModel(Path.GetFileName(x.AbsoluteUri), x.AbsoluteUri));

            _assets = assets is not null ? new ObservableCollection<AssetModel>(assets) : new();

            SelectedAsset = _assets.FirstOrDefault(x => x.Path.Contains("LottieLogo1.json"));
        }

        private readonly ObservableCollection<AssetModel> _assets;

        public IReadOnlyList<AssetModel> Assets => _assets;

        [ObservableProperty]
        public partial AssetModel? SelectedAsset { get; set; }

        [ObservableProperty]
        public partial bool EnableCheckerboard { get; set; }

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

    public partial class AssetModel : ViewModelBase
    {
        public AssetModel(string name, string path)
        {
            Name = name;
            Path = path;
        }

        [ObservableProperty]
        public partial string Name { get; set; }

        [ObservableProperty]
        public partial string Path { get; set; }

        public override string ToString() => Name;
    }
}
