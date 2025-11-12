using System;
using System.IO;
using System.Threading;
using Avalonia.Labs.Gif.Decoding;
using Avalonia.Platform;

namespace Avalonia.Labs.Gif
{
    /// <summary>
    /// Provides a GIF image source backed by a stream, enabling loading and access to GIF data from streams or resource
    /// URIs.
    /// </summary>
    /// <remarks>Use this class to wrap a stream containing GIF data for decoding and display. Instances can
    /// be created from an existing stream, a resource URI, or a URI string. The class implements <see
    /// cref="IDisposable"/>; callers should dispose the instance when the underlying stream is no longer needed to
    /// release resources.</remarks>
    public class GifStreamSource : IGifSource, IDisposable
    {
        private readonly Stream stream;

        private readonly PixelSize size;

        private bool disposedValue;

        private GifStreamSource(Stream stream)
        {
            using (GifDecoder decoder = new(stream, CancellationToken.None))
            {
                this.stream = stream;
                this.size = decoder.Size;
            }
        }

        /// <inheritdoc/>
        public PixelSize Size => size;

        /// <inheritdoc/>
        public Stream GetStream()
        {
            stream.Position = 0;
            return stream;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrap a stream as a GIF source.
        /// </summary>
        /// <param name="stream">The source stream</param>
        /// <returns>The wrapped instance</returns>
        /// <exception cref="InvalidGifStreamException"/>
        public static GifStreamSource FromStream(Stream stream)
        {
            return new GifStreamSource(stream);
        }

        /// <summary>
        /// Load a stream from a resource URI and wrap it as a GIF source.
        /// </summary>
        /// <param name="uri">The resource URI</param>
        /// <param name="baseUri">The base URI to use if uri is relative</param>
        /// <returns>The wrapped instance</returns>
        /// <exception cref="InvalidGifStreamException"/>
        public static GifStreamSource FromUri(Uri uri, Uri? baseUri = null)
        {
            return new GifStreamSource(AssetLoader.Open(uri, baseUri));
        }

        /// <summary>
        /// Load a stream from a resource URI and wrap it as a GIF source.
        /// </summary>
        /// <param name="uriString">The absolute resource URI string</param>
        /// <returns>The wrapped instance</returns>
        /// <exception cref="InvalidGifStreamException"/>
        /// <exception cref="ArgumentException"/>
        public static GifStreamSource FromUriString(string uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
            {
                return new GifStreamSource(AssetLoader.Open(uri));
            }
            else
            {
                throw new ArgumentException("Invalid absolute URI string.", nameof(uriString));
            }
        }
    }
}
