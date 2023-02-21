using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage : TemplatedControl
    {
        protected Image? ImagePart { get; private set; }
        protected Image? PlaceholderPart { get; private set; }

        private bool _isInitialized;
        private CancellationTokenSource? _tokenSource;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            ImagePart = e.NameScope.Get<Image>("PART_Image");
            PlaceholderPart = e.NameScope.Get<Image>("PART_PlaceholderImage");

            _isInitialized = true;

            if(Source != null)
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

            SetLoading();

            if (Source is IImage image)
            {
                AttachSource(image);

                return;
            }

            var uri = Source as Uri;

            if(uri == null)
            {
                var path = Source as string ?? source.ToString();

                if(!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri))
                {
                    SetFailed();

                    RaiseEvent(new AsyncImageFailedEventArgs(new UriFormatException($"Invalid Uri: {path}")));

                    return;
                }
            }

            if (uri != null)
            {
                if (uri.IsAbsoluteUri && (uri.Scheme == "http" || uri.Scheme == "https"))
                {
                    // Android doesn't allow network requests on the main thread, even though we are using async apis.
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await LoadImageAsync(uri, _tokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            SetFailed();

                            RaiseEvent(new AsyncImageFailedEventArgs(ex));
                        }
                    });
                }
                else if (File.Exists(uri.AbsolutePath))
                {
                    AttachSource(new Bitmap(uri.AbsolutePath));
                }
            }
            else
            {
                try
                {
                    var path = source.ToString();
                    uri = new Uri(path!, path!.StartsWith("/") ? UriKind.Relative : UriKind.RelativeOrAbsolute);
                    var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

                    AttachSource(new Bitmap(assetLoader!.Open(uri)));
                }
                catch(Exception ex)
                {
                    SetFailed();

                    RaiseEvent(new AsyncImageFailedEventArgs(ex));
                }
            }
        }

        private void AttachSource(IImage? image)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (ImagePart != null)
                {
                    ImagePart.Source = image;
                }

                if (image == null)
                {
                    SetUnLoaded();
                }
                else if (!image.Size.IsDefault)
                {
                    SetLoaded();

                    RaiseEvent(new Interactivity.RoutedEventArgs(OpenedEvent));
                }
            });
        }

        private void SetLoading()
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Classes.Remove("loaded");
                this.Classes.Remove("failed");
                this.Classes.Add("loading");
            });
        }

        private void SetLoaded()
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Classes.Add("loaded");
                this.Classes.Remove("failed");
                this.Classes.Remove("loading");
            });
        }

        private void SetUnLoaded()
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Classes.Remove("loaded");
                this.Classes.Remove("failed");
                this.Classes.Remove("loading");
            });
        }

        private void SetFailed()
        {
            Dispatcher.UIThread.Post(() =>
            {
                this.Classes.Remove("loaded");
                this.Classes.Add("failed");
                this.Classes.Remove("loading");
            });
        }

        private async Task LoadImageAsync(Uri url, CancellationToken token)
        {
            if(url != null)
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
                AttachSource(new Bitmap(memoryStream));
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == SourceProperty)
            {
                SetSource(Source);
            }
        }
    }
}
