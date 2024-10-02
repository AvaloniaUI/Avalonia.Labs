using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Platform;
using System.Linq;
using System.IO;
using Avalonia.Media;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class GifViewModel : ViewModelBase
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

        private AssetModel? _selectedAsset;
        private bool _enableCheckerboard;
        private int _iterationCount;
        private IReadOnlyList<Stretch>? _stretches;
        private Stretch? _selectedStretch;

        public IReadOnlyList<AssetModel> Assets => _assets;

        public AssetModel? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAsset, value);
            }
        }

        public bool EnableCheckerboard
        {
            get => _enableCheckerboard;
            set
            {
                this.RaiseAndSetIfChanged(ref _enableCheckerboard, value);
            }
        }


        public int IterationCount
        {
            get => _iterationCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _iterationCount, value);
            }
        }
        
        

        public IReadOnlyList<Stretch>? Stretches
        {
            get => _stretches;
            set => this.RaiseAndSetIfChanged(ref _stretches, value);
        }

        public Stretch? SelectedStretch
        {
            get => _selectedStretch;
            set => this.RaiseAndSetIfChanged(ref _selectedStretch, value);
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
}
