using System;
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
    }
}
