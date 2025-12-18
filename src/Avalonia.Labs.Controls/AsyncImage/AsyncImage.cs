using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
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

        private async void SetSource(object? source)
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

            Uri? uri = null;

            if (source is Uri sourceUri)
            {
                uri = sourceUri;
            } 
            else if (Source is string sourceString)
            {
                uri = new Uri(sourceString);
            }
            
            if (uri != null && uri.IsAbsoluteUri)
            {
                if (uri.Scheme == "http" || uri.Scheme == "https")
                {
                    Bitmap? bitmap = null;
                    // Android doesn't allow network requests on the main thread, even though we are using async apis.
#if NET6_0_OR_GREATER
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
                                await Dispatcher.UIThread.InvokeAsync(() => {
                                    State = AsyncImageState.Failed;

                                    RaiseEvent(new AsyncImageFailedEventArgs(ex));
                                });
                            }
                        });
                    }
                    else
#endif
                    {
                        try
                        {
                            bitmap = await LoadImageAsync(uri, _tokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            await Dispatcher.UIThread.InvokeAsync(() => {
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
                        Bitmap img = new(AssetLoader.Open(uri));
                        Bitmap scaledimg = await ScaleImageAsync(WithScale(uri, ScaleWidth, ScaleHeight), img);
                        AttachSource(scaledimg);
                    }
                    catch (Exception ex)
                    {
                        State = AsyncImageState.Failed;

                        RaiseEvent(new AsyncImageFailedEventArgs(ex));
                    }
                }
                else if (uri.Scheme == "file" && File.Exists(uri.LocalPath))
                {
                    Bitmap img = new(uri.LocalPath);
                    Bitmap scaledimg = await ScaleImageAsync(WithScale(uri, ScaleWidth, ScaleHeight), img);
                    AttachSource(scaledimg);
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

            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();

            if (image == null)
            {
                State = AsyncImageState.Unloaded;

                ImageTransition?.Start(ImagePart, PlaceholderPart, true, _tokenSource.Token);
            }
            else if (image.Size != default)
            {
                State = AsyncImageState.Loaded;

                ImageTransition?.Start(PlaceholderPart, ImagePart, true, _tokenSource.Token);

                RaiseEvent(new Interactivity.RoutedEventArgs(OpenedEvent));
            }
        }

        private async Task<Bitmap> LoadImageAsync(Uri? url, CancellationToken token)
        {

            Uri scaledUri = WithScale(url!, ScaleWidth, ScaleHeight);

            if (await ImageCache.Instance.IsUriCachedAsync(scaledUri))
                return (await ProvideCachedResourceAsync(scaledUri, token))!;


#if NET6_0_OR_GREATER
            using var client = new HttpClient();
            var stream = await client.GetStreamAsync(url, token).ConfigureAwait(false);

            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, token).ConfigureAwait(false);
#elif NETSTANDARD2_0
            using var client = new HttpClient();
            var response = await client.GetAsync(url, token).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
#endif

            memoryStream.Position = 0;

            Bitmap image = new(memoryStream);


            return await ScaleImageAsync(url!, image);
        }

        private async Task<Bitmap> ScaleImageAsync(Uri uri, Bitmap image)
        {
            int originalWidth = image.PixelSize.Width;
            int originalHeight = image.PixelSize.Height;

            (int targetWidth, int targetHeight) = CalculateTargetDimensions(originalWidth, originalHeight);

            Uri scaledUri = WithScale(uri, ScaleWidth, ScaleHeight);

            // No scaling
            if (ScaleHeight == null && ScaleWidth == null) return image;


            var constraints = new PixelSize(targetWidth, targetHeight);
            Bitmap scaledImage = image.CreateScaledBitmap(constraints);

            await ImageCache.Instance.SaveToInMemoryCacheAsync(scaledImage, scaledUri);
            return scaledImage;
        }


        public Uri WithScale(Uri baseUri, int? width = null, int? height = null)
        {
            var ub = new UriBuilder(baseUri);
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["scaleW"] = width?.ToString();
            query["scaleH"] = height?.ToString();
            ub.Query = query.ToString();
            return ub.Uri;
        }

        private (int width, int height) CalculateTargetDimensions(int origW, int origH)
        {
            if (ScaleWidth.HasValue && ScaleHeight.HasValue)
            {
                return (ScaleWidth.Value, ScaleHeight.Value);
            }

            if (ScaleWidth.HasValue)
            {
                double ratio = (double)origH / origW;
                int h = (int)Math.Ceiling(ScaleWidth.Value * ratio);
                return (ScaleWidth.Value, h);
            }

            if (ScaleHeight.HasValue)
            {
                double ratio = (double)origW / origH;
                int w = (int)Math.Ceiling(ScaleHeight.Value * ratio);
                return (w, ScaleHeight.Value);
            }

            return (origW, origH);
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
            if(IsCacheEnabled && imageUri != null)
            {
                return await ImageCache.Instance.GetFromCacheAsync(imageUri, cancellationToken: token).ConfigureAwait(false);
            }
            return null;
        }
    }
}
