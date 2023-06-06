using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Platform;
using System.Linq;
using System.IO;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class LottieViewModel : ViewModelBase
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

        private AssetModel? _selectedAsset;
        private bool _enableCheckerboard;

        public IReadOnlyList<AssetModel> Assets => _assets;

        public AssetModel? SelectedAsset
        {
            get => _selectedAsset; set
            {
                this.RaiseAndSetIfChanged(ref _selectedAsset, value);
            }
        }
        public bool EnableCheckerboard
        {
            get => _enableCheckerboard; set
            {
                this.RaiseAndSetIfChanged(ref _enableCheckerboard, value);
            }
        }

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
        private string _name;
        private string _path;

        public AssetModel(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name
        {
            get => _name; set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }
        public string Path
        {
            get => _path; set
            {
                this.RaiseAndSetIfChanged(ref _path, value);
            }
        }

        public override string ToString() => Name;
    }
}
