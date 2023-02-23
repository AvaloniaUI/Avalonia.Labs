using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage : TemplatedControl
    {
        protected Image? ImagePart { get; private set; }
        protected Image? PlaceholderPart { get; private set; }

        private bool _isInitialized;
        private CancellationTokenSource? _tokenSource;

        internal static readonly StyledProperty<AsyncImageState> StateProperty = AvaloniaProperty.Register<AsyncImage, AsyncImageState>(nameof(State));

        internal AsyncImageState State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            ImagePart = e.NameScope.Get<Image>("PART_Image");
            PlaceholderPart = e.NameScope.Get<Image>("PART_PlaceholderImage");

            _isInitialized = true;

            if (Source != null)
            {
                SetSource(Source);
            }
        }

        private async void SetSource(object source)
        {
            if (!_isInitialized)
            {
                return;
            }

            _tokenSource?.Cancel();

            _tokenSource = new CancellationTokenSource();

            AttachSource(null);

            if (source == null)
            {
                return;
            }

            State = AsyncImageState.Loading;

            if (Source is IImage image)
            {
                AttachSource(image);

                return;
            }

            if (Source == null)
            {
                return;
            }

            var uri = Source;

            if (uri != null && uri.IsAbsoluteUri)
            {
                if (uri.Scheme == "http" || uri.Scheme == "https")
                {
                    Bitmap? bitmap = null;
                    // Android doesn't allow network requests on the main thread, even though we are using async apis.
                    if (OperatingSystem.IsAndroid())
                    {
                        await Task.Run(async () =>
                        {
                            try
                            {
                                bitmap = await LoadImageAsync(uri, _tokenSource.Token);
                            }
                            catch (Exception ex)
                            {
                                State = AsyncImageState.Failed;

                                RaiseEvent(new AsyncImageFailedEventArgs(ex));
                            }
                        });
                    }
                    else
                    {
                        try
                        {
                            bitmap = await LoadImageAsync(uri, _tokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            State = AsyncImageState.Failed;

                            RaiseEvent(new AsyncImageFailedEventArgs(ex));
                        }
                    }

                    AttachSource(bitmap);
                }
                else if (uri.Scheme == "avares" || uri.Scheme == "resm")
                {
                    try
                    {
                        var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

                        AttachSource(new Bitmap(assetLoader!.Open(uri)));
                    }
                    catch (Exception ex)
                    {
                        State = AsyncImageState.Failed;

                        RaiseEvent(new AsyncImageFailedEventArgs(ex));
                    }
                }
                else if (uri.Scheme == "file" && File.Exists(uri.LocalPath))
                {
                    AttachSource(new Bitmap(uri.LocalPath));
                }
                else
                {
                    RaiseEvent(new AsyncImageFailedEventArgs(new UriFormatException($"Uri has unsupported scheme. Uri:{source}")));
                }
            }
            else
            {
                RaiseEvent(new AsyncImageFailedEventArgs(new UriFormatException($"Relative paths aren't supported. Uri:{source}")));
            }
        }

        private void AttachSource(IImage? image)
        {
            if (ImagePart != null)
            {
                ImagePart.Source = image;
            }

            if (image == null)
            {
                State = AsyncImageState.Unloaded;
            }
            else if (!image.Size.IsDefault)
            {
                State = AsyncImageState.Loaded;

                RaiseEvent(new Interactivity.RoutedEventArgs(OpenedEvent));
            }
        }

        private async Task<Bitmap> LoadImageAsync(Uri? url, CancellationToken token)
        {
#if NET6_0_OR_GREATER
            using var client = new HttpClient();
            var stream = await client.GetStreamAsync(url, token);

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
#elif NETSTANDARD2_0
            using var client = new WebClient();
            var data = await client.DownloadDataTaskAsync(url);
            using var memoryStream = new MemoryStream(data);
#endif

            memoryStream.Position = 0;
            return new Bitmap(memoryStream);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SourceProperty)
            {
                SetSource(Source);
            }
        }
    }
}
