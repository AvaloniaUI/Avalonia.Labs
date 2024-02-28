using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls.Cache;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// An image control that asynchronously retrieves an image using a <see cref="Uri"/>.
    /// </summary>
    [TemplatePart("PART_Image", typeof(Image))]
    [TemplatePart("PART_PlaceholderImage", typeof(Image))]
    public partial class AsyncImage : TemplatedControl
    {
        protected Image? ImagePart { get; private set; }
        protected Image? PlaceholderPart { get; private set; }

        private bool _isInitialized;
        private CancellationTokenSource? _tokenSource;
        private AsyncImageState _state;



        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {

            base.OnApplyTemplate(e);

            ImagePart = e.NameScope.Get<Image>("PART_Image");
            PlaceholderPart = e.NameScope.Get<Image>("PART_PlaceholderImage");

            _tokenSource = new CancellationTokenSource();

            _isInitialized = true;

            if (Source != null)
            {
                SetSource(Source);
            }
        }

        private bool _shouldAnimate;
        private CancellationTokenSource? _currentTransition;

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
                                await Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    State = AsyncImageState.Failed;

                                    RaiseEvent(new AsyncImageFailedEventArgs(ex));
                                });
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
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                State = AsyncImageState.Failed;

                                RaiseEvent(new AsyncImageFailedEventArgs(ex));
                            });
                        }
                    }

                    AttachSource(bitmap);
                }
                else if (uri.Scheme == "avares")
                {
                    try
                    {
                        AttachSource(new Bitmap(AssetLoader.Open(uri)));
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

        protected override Size ArrangeOverride(Size finalSize)
        {
            var result = base.ArrangeOverride(finalSize);

            if (_shouldAnimate)
            {
                _currentTransition?.Cancel();

                if (ImageTransition is { } transition)
                {

                    _shouldAnimate = false;

                    CancellationTokenSource cancel = new();
                    _currentTransition = cancel;


                    var from = _currentImage is not null ? PlaceholderPart : ImagePart;
                    var to = _currentImage is not null ? ImagePart : PlaceholderPart;



                    transition.Start(from, to, true, cancel.Token).ContinueWith((x) =>
                    {
                        if (!cancel.IsCancellationRequested)
                        {

                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                _shouldAnimate = false;
            }

            return result;
        }

        private IImage? _currentImage;

        private void AttachSource(IImage? image)
        {
            if (PlaceholderPart is null || ImagePart is null)
                return;

            _currentImage = image;

            ImagePart.Source = image;

            if (ImageTransition is not null)
            {
                _shouldAnimate = true;
                InvalidateArrange();
            }
            else
            {
                if (image is null)
                {
                    PlaceholderPart.Opacity = 1;
                    ImagePart.Opacity = 0;
                }
                else
                {
                    PlaceholderPart.Opacity = 0;
                    ImagePart.Opacity = 1;
                }
            }


        }



        private async Task<Bitmap> LoadImageAsync(Uri? url, CancellationToken token)
        {
            if (await ProvideCachedResourceAsync(url, token) is { } bitmap)
            {
                return bitmap;
            }
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

        protected virtual async Task<Bitmap?> ProvideCachedResourceAsync(Uri? imageUri, CancellationToken token)
        {
            if (IsCacheEnabled && imageUri != null)
            {
                return await ImageCache.Instance.GetFromCacheAsync(imageUri);
            }
            return null;
        }
    }
}
