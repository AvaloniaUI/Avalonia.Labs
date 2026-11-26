using System;
using System.Net.Http;

namespace Avalonia.Labs.Controls.Cache
{
    public class CacheOptions
    {
        private static CacheOptions? _cacheOptions;

        public static CacheOptions Default => _cacheOptions ??= new CacheOptions();
        public static void SetDefault(CacheOptions defaultCacheOptions)
        {
            _cacheOptions = defaultCacheOptions;
        }

        public string? BaseCachePath { get; set; }
        public TimeSpan? CacheDuration { get; set; }

        /// <summary>Gets or sets the client used to download http(s) images, e.g. to customize request headers.</summary>
        /// <remarks>When <see langword="null"/>, a default client is used. The cache never disposes it.</remarks>
        public HttpClient? HttpClient { get; set; }
    }
}
