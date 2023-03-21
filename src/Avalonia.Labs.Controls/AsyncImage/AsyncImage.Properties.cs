using System;
using Avalonia.Animation;
using Avalonia.Media;

namespace Avalonia.Labs.Controls
{
    public partial class AsyncImage
    {
        /// <summary>
        /// Defines the <see cref="PlaceholderSource"/> property.
        /// </summary>
        public static readonly StyledProperty<IImage> PlaceholderSourceProperty =
            AvaloniaProperty.Register<AsyncImage, IImage>(nameof(PlaceholderSource));

        /// <summary>
        /// Defines the <see cref="Source"/> property.
        /// </summary>
        public static readonly StyledProperty<Uri> SourceProperty =
            AvaloniaProperty.Register<AsyncImage, Uri>(nameof(Source));

        /// <summary>
        /// Defines the <see cref="Stretch"/> property.
        /// </summary>
        public static readonly StyledProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<AsyncImage, Stretch>(nameof(Stretch), Stretch.Uniform);

        /// <summary>
        /// Defines the <see cref="PlaceholderStretch"/> property.
        /// </summary>
        public static readonly StyledProperty<Stretch> PlaceholderStretchProperty =
            AvaloniaProperty.Register<AsyncImage, Stretch>(nameof(PlaceholderStretch), Stretch.Uniform);

        /// <summary>
        /// Defines the <see cref="State"/> property.
        /// </summary>
        public static readonly DirectProperty<AsyncImage, AsyncImageState> StateProperty = AvaloniaProperty.RegisterDirect<AsyncImage, AsyncImageState>(nameof(State),
            o => o.State,
            (o, v) => o.State = v);

        /// <summary>
        /// Defines the <see cref="ImageTransition"/> property.
        /// </summary>
        public static readonly StyledProperty<IPageTransition?> ImageTransitionProperty =
            AvaloniaProperty.Register<AsyncImage, IPageTransition?>(nameof(ImageTransition),
            new CrossFade(TimeSpan.FromSeconds(0.25)));

        /// <summary>
        /// Defines the <see cref="IsCacheEnabled"/> property.
        /// </summary>
        public static readonly DirectProperty<AsyncImage,bool> IsCacheEnabledProperty =
            AvaloniaProperty.RegisterDirect<AsyncImage, bool>(nameof(IsCacheEnabled), o => o.IsCacheEnabled, (o, v) => o.IsCacheEnabled = v);
        private bool _isCacheEnabled;

        /// <summary>
        /// Gets or sets the placeholder image.
        /// </summary>
        public IImage PlaceholderSource
        {
            get => GetValue(PlaceholderSourceProperty);
            set => SetValue(PlaceholderSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the uri pointing to the image resource
        /// </summary>
        public Uri Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Gets or sets a value controlling how the image will be stretched.
        /// </summary>
        public Stretch Stretch
        {
            get { return GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value controlling how the placeholder will be stretched.
        /// </summary>
        public Stretch PlaceholderStretch
        {
            get { return GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Gets the current loading state of the image.
        /// </summary>
        public AsyncImageState State
        {
            get => _state;
            private set => SetAndRaise(StateProperty, ref _state, value);
        }

        /// <summary>
        /// Gets or sets the transition to run when the image is loaded.
        /// </summary>
        public IPageTransition? ImageTransition
        {
            get => GetValue(ImageTransitionProperty);
            set => SetValue(ImageTransitionProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to use cache for retrieved images
        /// </summary>
        public bool IsCacheEnabled
        {
            get => _isCacheEnabled;
            set => SetAndRaise(IsCacheEnabledProperty, ref _isCacheEnabled, value);
        }
    }
}
